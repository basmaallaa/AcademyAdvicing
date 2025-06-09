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
using ClosedXML.Excel;

namespace AcademyAdvicingGp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AcademyContext _academyContext;

        public StudentsController(IStudentService studentService, IUnitOfWork unitOfWork, AcademyContext academyContext)
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

        /*الاكسل شيت */
        [HttpGet("export-students")]
        [Authorize(Roles = "StudentAffair")]
        public async Task<IActionResult> ExportStudents()
        {
            var students = await _academyContext.Students.ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Students");

            // Set headers
            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "AdmissionYear";
            worksheet.Cell(1, 4).Value = "UserName";
            worksheet.Cell(1, 5).Value = "Email";
            worksheet.Cell(1, 6).Value = "Level";
            worksheet.Cell(1, 7).Value = "Status";
            worksheet.Cell(1, 8).Value = "PhoneNumber";
            worksheet.Cell(1, 9).Value = "ImagePath";

            int row = 2;
            foreach (var student in students)
            {
                worksheet.Cell(row, 1).Value = student.Id;
                worksheet.Cell(row, 2).Value = student.Name;
                worksheet.Cell(row, 3).Value = student.AdmissionYear;
                worksheet.Cell(row, 4).Value = student.UserName;
                worksheet.Cell(row, 5).Value = student.Email;
                worksheet.Cell(row, 6).Value = student.Level.ToString();   // أو (int)student.Level لو عايز رقم
                worksheet.Cell(row, 7).Value = student.Status.ToString();  // أو (int)student.Status
                worksheet.Cell(row, 8).Value = student.PhoneNumber;
                worksheet.Cell(row, 9).Value = student.ImagePath;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Students_{DateTime.Now:yyyyMMdd}.xlsx");
        }



        [HttpPost("import-students")]
        [Authorize(Roles = "StudentAffair")]  // مثلاً لو حابب تحدد صلاحية
        public async Task<IActionResult> ImportStudents(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // تخطي الصف الأول لو فيه عنوان

            var students = new List<Student>();

            foreach (var row in rows)
            {
                try
                {
                    var student = new Student
                    {
                        // لا تعيّن Id هنا، فقط بقية الحقول
                        // Id = row.Cell(1).GetValue<int>(),  <- احذف هذا السطر
                        Name = row.Cell(2).GetValue<string>(),
                        AdmissionYear = row.Cell(3).GetValue<string>(),
                        UserName = row.Cell(4).GetValue<string>(),
                        Email = row.Cell(5).GetValue<string>(),
                        Level = Enum.Parse<Academy.Core.Enums.Levels>(row.Cell(6).GetValue<string>()),
                        Status = Enum.Parse<Academy.Core.Enums.Status>(row.Cell(7).GetValue<string>()),
                        PhoneNumber = row.Cell(8).GetValue<string>(),
                        ImagePath = row.Cell(9).GetValue<string>()
                    };


                    students.Add(student);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Error processing row {row.RowNumber()}: {ex.Message}");
                }
            }

            foreach (var student in students)
            {
                var existingStudent = await _academyContext.Students.FindAsync(student.Id);
                if (existingStudent == null)
                {
                    _academyContext.Students.Add(student);
                }
                else
                {
                    existingStudent.Name = student.Name;
                    existingStudent.AdmissionYear = student.AdmissionYear;
                    existingStudent.UserName = student.UserName;
                    existingStudent.Email = student.Email;
                    existingStudent.Level = student.Level;
                    existingStudent.Status = student.Status;
                    existingStudent.PhoneNumber = student.PhoneNumber;
                    existingStudent.ImagePath = student.ImagePath;
                }
            }

            await _academyContext.SaveChangesAsync();

            return Ok($"{students.Count} students imported successfully.");
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



        //[HttpGet("available-courses")]
        //[Authorize(Roles = "Student")]
        //public async Task<IActionResult> GetAvailableCourses()
        //{
        //    var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        //    if (string.IsNullOrEmpty(userEmail))
        //        return Unauthorized("Email not found in token.");

        //    var student = await _unitOfWork.Repository<Student>()
        //        .GetAllIncludingAsyncc(s => s.Courses)
        //        .ContinueWith(t => t.Result.FirstOrDefault(s => s.Email == userEmail));

        //    if (student == null)
        //        return NotFound("Student not found");

        //    DateTime now = DateTime.Now;
        //    int currentYear = now.Year;
        //    string academicYear = $"{currentYear}/{currentYear + 1}";

        //    Semster semester;
        //    if (now.Month >= 9 && now.Month <= 12)
        //        semester = Semster.Fall;
        //    else if (now.Month >= 1 && now.Month <= 8)
        //        semester = Semster.Spring;
        //    else
        //        return BadRequest("No available courses in summer (June–August).");

        //    int parsedLevel = (int)student.Level;

        //    var availableCourses = await _unitOfWork.Repository<AvailableCourse>()
        //        .GetAllIncludingAsyncc(ac => ac.Course, ac => ac.Doctor);

        //    var studentAssignedCourses = student.Courses ?? new List<AssignedCourse>();
        //    var eligibleCourses = new List<object>();

        //    foreach (var ac in availableCourses)
        //    {
        //        if (ac.Semester != semester || ac.AcademicYears != academicYear)
        //            continue;

        //        // الشرط الخاص بالمستوى
        //        int courseLevel = (int)ac.Level;

        //        if (courseLevel != parsedLevel)
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            // المادة بدون level → امنعها فقط على طلاب سنة أولى
        //            if (parsedLevel == 0)
        //                continue;
        //        }

        //        bool isAlreadyAssigned = studentAssignedCourses.Any(sc => sc.CourseId == ac.CourseId);

        //        if (parsedLevel == 0) // سنة أولى
        //        {
        //            if (!isAlreadyAssigned)
        //            {
        //                eligibleCourses.Add(new
        //                {
        //                    CourseName = ac.Course?.Name,
        //                    CreditHours = ac.Course?.CreditHours,
        //                    Credit = ac.Course?.Credit
        //                });
        //            }
        //        }
        //        else
        //        {
        //            // لباقي المستويات (2،3،4)
        //            bool hasPassedPrerequisite = true;

        //            if (ac.Course.PrerequisiteCourseId.HasValue)
        //            {
        //                var prerequisiteId = ac.Course.PrerequisiteCourseId.Value;
        //                hasPassedPrerequisite = studentAssignedCourses.Any(sc =>
        //                       sc.CourseId == prerequisiteId && sc.TotalGrades >= 50);

        //            }

        //            if (hasPassedPrerequisite && !isAlreadyAssigned)
        //            {
        //                eligibleCourses.Add(new
        //                {
        //                    CourseName = ac.Course?.Name,
        //                    CreditHours = ac.Course?.CreditHours,
        //                    Credit = ac.Course?.Credit
        //                });
        //            }
        //        }
        //    }

        //    return Ok(eligibleCourses);
        //}

        [HttpGet("available-courses")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetAvailableCourses()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("Email not found in token.");

            var student = await _unitOfWork.Repository<Student>()
                .GetAllIncludingAsyncc(s => s.Courses)
                .ContinueWith(t => t.Result.FirstOrDefault(s => s.Email == userEmail));

            if (student == null)
                return NotFound("Student not found");

            DateTime now = DateTime.Now;
            int currentYear = now.Year;
            string academicYear = $"{currentYear}/{currentYear + 1}";

            Semster semester;
            if (now.Month >= 9 && now.Month <= 12)
                semester = Semster.Fall;
            else if (now.Month >= 1 && now.Month <= 8)
                semester = Semster.Spring;
            else
                return BadRequest("No available courses in summer (June–August).");

            int parsedLevel = (int)student.Level;

            var availableCourses = await _unitOfWork.Repository<AvailableCourse>()
                .GetAllIncludingAsyncc(ac => ac.Course, ac => ac.Doctor);

            var studentAssignedCourses = student.Courses ?? new List<AssignedCourse>();
            var eligibleCourses = new List<object>();

            foreach (var ac in availableCourses)
            {
                if (ac.Semester != semester || ac.AcademicYears != academicYear)
                    continue;

                bool isAlreadyAssigned = studentAssignedCourses.Any(sc => sc.CourseId == ac.CourseId);

                if (!ac.Level.HasValue)
                {
                    // المادة بدون level → امنعها فقط على طلاب سنة أولى
                    if (parsedLevel == 0)
                        continue;
                }
                else
                {
                    int courseLevel = (int)ac.Level.Value;

                    if (courseLevel != parsedLevel)
                        continue;
                }

                if (parsedLevel == 0) // سنة أولى
                {
                    if (!isAlreadyAssigned)
                    {
                        eligibleCourses.Add(new
                        {
                            CourseName = ac.Course?.Name,
                            CreditHours = ac.Course?.CreditHours,
                            Credit = ac.Course?.Credit
                        });
                    }
                }
                else
                {
                    bool hasPassedPrerequisite = true;

                    if (ac.Course.PrerequisiteCourseId.HasValue)
                    {
                        var prerequisiteId = ac.Course.PrerequisiteCourseId.Value;
                        hasPassedPrerequisite = studentAssignedCourses.Any(sc =>
                            sc.CourseId == prerequisiteId && sc.TotalGrades >= 50);
                    }

                    if (hasPassedPrerequisite && !isAlreadyAssigned)
                    {
                        eligibleCourses.Add(new
                        {
                            CourseName = ac.Course?.Name,
                            CreditHours = ac.Course?.CreditHours,
                            Credit = ac.Course?.Credit
                        });
                    }
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

            foreach (var assignedCourse in student.Courses)
            {
                await _academyContext.Entry(assignedCourse).Reference(ac => ac.Course).LoadAsync();
            }

            var availableCourse = await _unitOfWork.Repository<AvailableCourse>()
                .GetIncludingAsync(
                    ac => ac.Id == availableCourseId,
                    ac => ac.Course,
                    ac => ac.Course.PrerequisiteCourse
                );

            if (availableCourse == null)
                return NotFound("Available course not found");

            student.Courses ??= new List<AssignedCourse>();

            bool alreadyRegistered = student.Courses.Any(c =>
                c.CourseId == availableCourse.CourseId &&
                c.AcademicYears == availableCourse.AcademicYears &&
                c.Semester == availableCourse.Semester);

            if (alreadyRegistered)
                return BadRequest("Student already registered this course in the same term and academic year.");

            DateTime now = DateTime.Now;
            string currentAcademicYear = $"{now.Year}/{now.Year + 1}";

            Semster currentSemester = (now.Month >= 9 && now.Month <= 12) ? Semster.Fall :
                                       (now.Month >= 1 && now.Month <= 8) ? Semster.Spring :
                                       throw new InvalidOperationException("Invalid time for course registration.");

            // استخراج مستوى الطالب كـ enum مباشرة
            Levels studentLevel = student.Level;

            if (availableCourse.Semester != currentSemester || availableCourse.AcademicYears != currentAcademicYear)
                return BadRequest("This course is not available in the current semester or academic year.");

            if (availableCourse.Level != studentLevel)
            {
                // المادة بدون level → امنعها فقط على طلاب سنة أولى
                if (studentLevel == Levels.One)
                    return BadRequest("You cannot register this course in level one.");
            }

            // التحقق من المتطلب السابق
            if (availableCourse.Course.PrerequisiteCourseId.HasValue && studentLevel != Levels.One)
            {
                var prereqId = availableCourse.Course.PrerequisiteCourseId.Value;
                bool hasPassed = student.Courses.Any(c =>
                    c.CourseId == prereqId && c.FinalScore >= 50);

                if (!hasPassed)
                    return BadRequest("You have not passed the prerequisite course.");
            }

            // التحقق من عدد الساعات
            var coursesThisTerm = student.Courses.Where(c =>
                c.AcademicYears == availableCourse.AcademicYears &&
                c.Semester == availableCourse.Semester).ToList();

            int currentHours = coursesThisTerm.Sum(c => c.Course?.CreditHours ?? 0);
            int courseCreditHours = availableCourse.Course.CreditHours;

            int maxAllowedHours = (studentLevel == Levels.One) ? 18 : (student.GPA < 2 ? 12 : 18);

            if (currentHours + courseCreditHours > maxAllowedHours)
                return BadRequest($"You cannot exceed {maxAllowedHours} credit hours in this semester.");

            // تسجيل المادة
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


        [HttpGet("my-assigned-courses")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyAssignedCourses()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("Email not found in token.");

            var student = await _academyContext.Students
                .Include(s => s.Courses)
                    .ThenInclude(ac => ac.Course)
                .FirstOrDefaultAsync(s => s.Email == userEmail);

            if (student == null)
                return NotFound("Student not found.");

            var assignedCourses = student.Courses.Select(c => new
            {
                AssignedCourseId = $"{c.StudentId}_{c.CourseId}",
                CourseId = c.CourseId,
                CourseName = c.Course?.Name,
                CourseCode = c.Course?.CourseCode,
                AcademicYear = c.AcademicYears,
                Semester = c.Semester.ToString(),
                ClassWorkScore = c.ClassWorkScore,
                PracticalScore = c.PracticalScore,
                FinalScore = c.FinalScore,
                Grade = c.Grade
            });

            return Ok(assignedCourses);
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
        public async Task<IActionResult> CalculateStudentGpaAndCreditHours()
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

            var gradePoints = new Dictionary<string, float>
    {
        { "A+", 4f },
        { "A", 3.75f },
        { "B+", 3.4f },
        { "B", 3.1f },
        { "C+", 2.8f },
        { "C", 2.5f },
        { "D+", 2.25f },
        { "D", 2f },
        { "F", 1f } // تمّت الإضافة هنا
    };

            float totalPoints = 0;
            int totalCreditHours = 0;

            foreach (var assignedCourse in studentEntity.Courses)
            {
                if (gradePoints.TryGetValue(assignedCourse.Grade, out float gradePoint))
                {
                    int creditHours = assignedCourse.Course.CreditHours;
                    totalPoints += gradePoint * creditHours;
                    totalCreditHours += creditHours;
                }
            }

            float gpa = totalCreditHours > 0 ? totalPoints / totalCreditHours : 0;
            gpa = (float)Math.Round(gpa, 2);

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