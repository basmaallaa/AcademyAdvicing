using Academy.Core;
using Academy.Core.Dtos;
using Academy.Core.Models;
using Academy.Core.ServicesInterfaces;
using Academy.Core.ServicesInterfaces.ICoursesInterface;
using Academy.Repo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Academy.Core.Enums;
using DocumentFormat.OpenXml.InkML;

namespace AcademyAdvicingGp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AcademyContext _academyContext;

        public StudentsController(IStudentService studentService , IUnitOfWork unitOfWork, AcademyContext academyContext)
        {
            _studentService = studentService;
            _unitOfWork = unitOfWork;
            _academyContext = academyContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllStudents()
        {
            var result = await _studentService.GetAllStudentsAsync();

            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(int? id)
        {
            if (id is null) return BadRequest("Invalid id !!");

            var result = await _studentService.GetStudentByIdAsync(id.Value);

            if (result is null) return NotFound($"The Student with Id: {id} not found :(  ");
            return Ok(result);
        }

        //[HttpPost]
        //public async Task<IActionResult> AddStudent([FromBody] StudentDto studentDto)
        //{
        //    if (studentDto == null)
        //        return BadRequest("Student data is required.");

        //    var addedStudent = await _studentService.AddStudentAsync(studentDto);
        //    return CreatedAtAction(nameof(GetAllStudents), addedStudent);
        //}

        [HttpPost("add")]
        [Authorize(Roles = "StudentAffair")] // السماح فقط لموظف شؤون الطلاب
        public async Task<IActionResult> AddStudent([FromForm] StudentDto model)
        {
            if (model == null)
                return BadRequest("Invalid student data.");

            try
            {
                var studentDto = await _studentService.AddStudentAsync(model);
                return Ok(studentDto);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentDtoID studentDtoid)
        {
            if (studentDtoid == null || id != studentDtoid.Id)
                return BadRequest("Invalid student data or ID mismatch.");

            var existingStudent = await _studentService.GetStudentByIdAsync(id);
            if (existingStudent == null)
                return NotFound($"Student with ID {id} not found.");

            await _studentService.UpdateStudentAsync(studentDtoid);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var existingStudent = await _studentService.GetStudentByIdAsync(id);
            if (existingStudent == null)
                return NotFound($"Student with ID {id} not found.");

            await _studentService.DeleteStudentAsync(id);
            return Ok();
        }



       
        [HttpGet("search")]
        public async Task<IActionResult> SearchStudents([FromQuery] string? searchTerm)
        {
            var students = await _studentService.SearchStudentsAsync(searchTerm);

            if (students == null || !students.Any())
            {
                return NotFound("No students found matching the search criteria.");
            }

            return Ok(students);
        }

        /* The student assign course that are available */




        [HttpGet("available-courses/{semester}/{academicYear}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetAvailableCourses(Semster semester, int academicYear)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("Email not found in token.");

            // Get student with assigned courses
            var student = await _unitOfWork.Repository<Student>()
                .GetAllIncludingAsyncc(s => s.Courses)
                .ContinueWith(t => t.Result.FirstOrDefault(s => s.Email == userEmail));

            if (student == null)
                return NotFound("Student not found");

            // Get the level of the student (automatically from the student data)
            string level = student.Level;

            // Get all available courses and include Course + Doctor
            var availableCourses = await _unitOfWork.Repository<AvailableCourse>()
                .GetAllIncludingAsyncc(ac => ac.Course, ac => ac.Doctor);

            var studentAssignedCourses = student.Courses ?? new List<AssignedCourse>();
            var eligibleCourses = new List<object>();

            // Filter available courses based on semester, academic year, and student's level
            foreach (var ac in availableCourses)
            {
                if (ac.Level != level || ac.Semester != semester || ac.AcademicYears != academicYear)
                    continue;

                bool isAlreadyAssigned = studentAssignedCourses.Any(sc => sc.CourseId == ac.CourseId);
                if (isAlreadyAssigned)
                    continue;

                bool hasPassedPrerequisite = true;

                // Check if the course has prerequisites and verify if the student has passed it
                if (!string.IsNullOrEmpty(ac.Course.prerequisite))
                {
                    var prerequisiteCourse = await _unitOfWork.Repository<Course>()
                        .GetAllAsync()
                        .ContinueWith(t => t.Result.FirstOrDefault(c =>
                            c.Name.Trim().ToLower() == ac.Course.prerequisite.Trim().ToLower()));

                    if (prerequisiteCourse == null)
                    {
                        hasPassedPrerequisite = false;
                    }
                    else
                    {
                        hasPassedPrerequisite = studentAssignedCourses
                            .Any(sc => sc.CourseId == prerequisiteCourse.CourseId && sc.FinalScore >= 50);
                    }
                }

                // Add eligible course to the list if prerequisites are met
                if (hasPassedPrerequisite)
                {
                    var firstDoctor = ac.Doctor;

                    eligibleCourses.Add(new
                    {
                        AvailableCourseId = ac.Id,
                        CourseName = ac.Course?.Name,
                        CourseId = ac.Course?.CourseId,
                        Prerequisite = ac.Course?.prerequisite,
                        Credit = ac.Course?.Credit,
                        CreditHours = ac.Course?.CreditHours,
                        Category = ac.Course?.category,
                        Type = ac.Course?.type,
                        DoctorName = firstDoctor?.Name ?? "Not Assigned",
                        DoctorId = firstDoctor?.Id ?? 0,
                        AcademicYear = ac.AcademicYears,
                        Semester = ac.Semester.ToString(),
                        Level = ac.Level
                    });
                }
            }

            return Ok(eligibleCourses);
        }





        [HttpPost("assign-course/{availableCourseId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> AssignCourseToStudent(int availableCourseId)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("Email not found in token.");

            var student = await _unitOfWork.Repository<Student>()
                .GetIncludingAsync(s => s.Email == userEmail, s => s.Courses);

            if (student == null)
                return NotFound("Student not found");

            var availableCourse = await _unitOfWork.Repository<AvailableCourse>()
                .GetIncludingAsync(ac => ac.Id == availableCourseId, ac => ac.Course);

            if (availableCourse == null)
                return NotFound("Available course not found");

            student.Courses ??= new List<AssignedCourse>();

            bool alreadyRegistered = student.Courses.Any(c => c.CourseId == availableCourse.CourseId);
            if (alreadyRegistered)
                return BadRequest("Student already registered this course");

            // عدد الكورسات اللي سجلها الطالب في نفس الترم والعام الأكاديمي
            int registeredCountThisTerm = student.Courses
                .Count(c => c.Semester == availableCourse.Semester && c.AcademicYears == availableCourse.AcademicYears);

            // تحويل Level لتجنب مشاكل الـ Case
            string studentLevel = student.Level?.Trim().ToLower();

            // حالة: سنة أولى + الترم الأول
            bool isLevelOneFirstSemester = studentLevel == "one" && availableCourse.Semester == 0;

            if (isLevelOneFirstSemester)
            {
                if (registeredCountThisTerm >= 6)
                    return BadRequest("You cannot register more than 6 courses in the first semester of level one.");
            }
            else
            {
                if (student.GPA < 2)
                {
                    if (registeredCountThisTerm >= 4)
                        return BadRequest("Your GPA is less than 2. You can only register up to 4 courses this semester.");
                }
                else // GPA >= 2
                {
                    if (registeredCountThisTerm >= 6)
                        return BadRequest("You cannot register more than 6 courses this semester.");
                }
            }

            // إضافة الكورس
            student.Courses.Add(new AssignedCourse
            {
                StudentId = student.Id,
                CourseId = availableCourse.CourseId,
                Grade = "0",
                AcademicYears = availableCourse.AcademicYears,
                Semester = availableCourse.Semester
            });

            await _unitOfWork.CompleteAsync();

            return Ok("Course assigned successfully");
        }



        [HttpGet("my-course-scores")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyCourseScores([FromQuery] int courseId)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("Email not found in token.");

            // جلب الطالب مع الكورسات اللي سجلها والكورس نفسه
            var student = await _academyContext.Students
                .Include(s => s.Courses)
                    .ThenInclude(sc => sc.Course)
                .FirstOrDefaultAsync(s => s.Email == userEmail);

            if (student == null)
                return NotFound("Student profile not found.");

            // التأكد إن الطالب سجل فعلاً الكورس ده
            var selectedCourse = student.Courses.FirstOrDefault(c => c.CourseId == courseId);
            if (selectedCourse == null)
                return BadRequest("You are not assigned to this course.");

            // تجهيز البيانات المطلوبة فقط
            return Ok(new
            {
                CourseId = selectedCourse.CourseId,
                CourseName = selectedCourse.Course?.Name,
                ClassWorkScore = selectedCourse.ClassWorkScore,
                PracticalScore = selectedCourse.PracticalScore
            });
        }

        [HttpPut("CalculateStudentGpaAndCreditHours")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CalculateStudentGpaAndCreditHours(int academicYear, Semster semester)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("User email not found.");

            var studentEntity = await _academyContext.Students
                .Where(s => s.Email == userEmail)
                .Include(s => s.Courses)
                    .ThenInclude(ac => ac.Course)
                .FirstOrDefaultAsync();

            if (studentEntity == null)
                return NotFound("Student not found.");

            var semesterInt = (int)semester; // نحول السيمستر اللي جاي من الريكوست إلى int

            var assignedCourses = studentEntity.Courses
                .Where(c => c.AcademicYears == academicYear &&
                            (int)c.Semester == semesterInt) // نحول سيمستر الكورس كمان إلى int
                .ToList();

            if (assignedCourses == null || assignedCourses.Count == 0)
            {
                return NotFound(new
                {
                    Message = "No assigned courses found for the given academic year and semester.",
                    StudentLevel = studentEntity.Level,
                    AcademicYear = academicYear,
                    SemesterRequested = semester.ToString()
                });
            }

            var gradePoints = new Dictionary<string, float>
    {
        { "A+", 4f },
        { "A", 3.75f },
        { "B+", 3.4f },
        { "B", 3.1f },
        { "C+", 2.8f },
        { "C", 2.5f },
        { "D+", 2.25f },
        { "D", 2f }
    };

            float totalPoints = 0;
            int totalCreditHours = 0;

            foreach (var assignedCourse in assignedCourses)
            {
                if (gradePoints.TryGetValue(assignedCourse.Grade, out float gradePoint))
                {
                    int creditHours = assignedCourse.Course.CreditHours;
                    totalPoints += gradePoint * creditHours;
                    totalCreditHours += creditHours;
                }
            }

            float gpa = totalCreditHours > 0 ? totalPoints / totalCreditHours : 0;

            studentEntity.GPA = gpa;
            studentEntity.CompeletedHours = totalCreditHours;

            await _academyContext.SaveChangesAsync();

            return Ok(new
            {
                GPA = gpa,
                CompletedHours = totalCreditHours,
                StudentLevel = studentEntity.Level
            });
        }






    }
}
        


