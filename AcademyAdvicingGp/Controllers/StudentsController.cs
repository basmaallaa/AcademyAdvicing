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




        //    [HttpGet("{studentId}/available-courses")]
        //    public async Task<IActionResult> GetAvailableCourses(int studentId)
        //    {
        //        var student = await _unitOfWork.Repository<Student>()
        //            .GetAllIncludingAsyncc(s => s.Courses)
        //            .ContinueWith(t => t.Result.FirstOrDefault(s => s.Id == studentId));

        //        if (student == null)
        //            return NotFound("Student not found");

        //        // Get the IDs of courses the student already completed
        //        var previouslyTakenCourseIds = student.Courses?
        //.Where(sc => !string.IsNullOrEmpty(sc.Grade)) // الطالب حصل على درجة
        //.Select(sc => sc.CourseId)
        //.ToList() ?? new List<int>();

        //        // Get all available courses including their Course navigation
        //        // بدل previouslyTakenCourseIds نخزن CourseCodes مش CourseIds
        //        var previouslyTakenCourseCodes = student.Courses?
        //            .Where(sc => sc.Grade != null) // يعني نجح فيها أو خلصها
        //            .Select(sc => sc.Course.CourseCode) // نفترض إن Course navigation موجود
        //            .ToList() ?? new List<string>();

        //        var availableCourses = await _unitOfWork.Repository<AvailableCourse>()
        //.GetAllIncludingAsyncc(ac => ac.Course);

        //        var eligibleCourses = availableCourses
        //            .Where(ac =>
        //                // مش مسجل بالفعل
        //                !(student.Courses?.Any(sc => sc.CourseId == ac.CourseId) ?? false)

        //                // مفيش prerequisite أو الطالب واخدها
        //                && (
        //                    string.IsNullOrEmpty(ac.Course.prerequisite)
        //                    || previouslyTakenCourseCodes.Contains(ac.Course.prerequisite)
        //                )
        //            ).ToList();


        //        return Ok(eligibleCourses);
        //    }


        [HttpGet("{studentId}/available-courses")]
        public async Task<IActionResult> GetAvailableCourses(int studentId)
        {
            var student = await _unitOfWork.Repository<Student>()
                .GetAllIncludingAsyncc(s => s.Courses)
                .ContinueWith(t => t.Result.FirstOrDefault(s => s.Id == studentId));

            if (student == null)
                return NotFound("Student not found");

            var previouslyTakenCourseCodes = student.Courses?
                .Where(sc => sc.Grade != null && sc.Course != null)
                .Select(sc => sc.Course.CourseCode)
                .ToList() ?? new List<string>();

            var availableCourses = await _unitOfWork.Repository<AvailableCourse>()
                .GetAllIncludingAsyncc(ac => ac.Course);

            var eligibleCourses = availableCourses
                .Where(ac =>
                    // مش مسجل بالفعل
                    !(student.Courses?.Any(sc => sc.CourseId == ac.CourseId) ?? false)

                    // مفيش prerequisite أو الطالب واخدها
                    && (
                        string.IsNullOrEmpty(ac.Course?.prerequisite)
                        || previouslyTakenCourseCodes.Contains(ac.Course.prerequisite)
                    )
                ).ToList();

            return Ok(eligibleCourses);
        }





        [HttpPost("{studentId}/assign-course/{availableCourseId}")]
        public async Task<IActionResult> AssignCourseToStudent(int studentId, int availableCourseId)
        {
            // 1. جيب الطالب من الداتا
            var student = await _unitOfWork.Repository<Student>()
                .GetIncludingAsync(s => s.Id == studentId, s => s.Courses); // تأكدنا إن الكورسات متحمّلة مع الطالب

            if (student == null)
                return NotFound("Student not found");

            // 2. جيب الكورس المتاح
            var availableCourse = await _unitOfWork.Repository<AvailableCourse>()
                .GetIncludingAsync(ac => ac.Id == availableCourseId, ac => ac.Course);

            if (availableCourse == null)
                return NotFound("Available course not found");

            // 3. تأكد إن الـ Courses مش null
            student.Courses ??= new List<AssignedCourse>();

            // 4. تأكد إن الطالب مش مسجل نفس الكورس قبل كده
            bool alreadyRegistered = student.Courses.Any(c => c.CourseId == availableCourse.CourseId);
            if (alreadyRegistered)
                return BadRequest("Student already registered this course");

            // 5. أضف الكورس للطالب
            student.Courses.Add(new AssignedCourse
            {
                StudentId = studentId,
                CourseId = availableCourse.CourseId,
                Grade = "0" // لسه متسجلش درجات
            });

            // 6. احفظ التغييرات
            await _unitOfWork.CompleteAsync();

            // 7. رجّع OK
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


    }
}
        


