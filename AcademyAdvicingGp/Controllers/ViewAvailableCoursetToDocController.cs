using Academy.Core;
using Academy.Core.Enums;
using Academy.Core.Models;
using Academy.Repo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ClosedXML.Excel;
using System.IO;

namespace AcademyAdvicingGp.Controllers
{
    public class ViewAvailableCoursetToDocController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AcademyContext _academyContext;

        public ViewAvailableCoursetToDocController(IUnitOfWork unitOfWork, AcademyContext academyContext)
        {
            _unitOfWork = unitOfWork;
            _academyContext = academyContext;
        }





        [HttpGet("View_Available_Course_TO_Doctor")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetDoctorCourses(int academicYear, Semster semester)
        {
            // استخراج الإيميل من الـ JWT token
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("Email not found in token.");

            // البحث عن الدكتور في جدول الأكاديمية بناءً على الإيميل
            var doctor = await _academyContext.Doctors
                .FirstOrDefaultAsync(d => d.Email == userEmail);

            if (doctor == null)
                return NotFound("Doctor profile not found for the current user.");

            // جلب الكورسات المرتبطة بالدكتور في السنة والفصل المحددين
            var courses = await _academyContext.Availablecourses
                .Where(ac => ac.DoctorId == doctor.Id &&
                             ac.AcademicYears == academicYear &&
                             ac.Semester == semester)
                .Include(ac => ac.Course)
                .ToListAsync();

            if (courses == null || !courses.Any())
                return NotFound("No assigned courses found for this doctor in the specified year and semester.");

            // تجهيز النتيجة للإرجاع
            var result = courses.Select(course => new
            {
                CourseId = course.Course.CourseId,
                Name = course.Course.Name,
                Code = course.Course.CourseCode,
                AcademicYear = course.AcademicYears,
                Semester = course.Semester
            });

            return Ok(result);
        }


        [HttpGet("course-students-with-grades")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetCourseStudentsWithGrades([FromQuery] int? availableCourseId, [FromQuery] int? academicYear, [FromQuery] Semster? semester)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("Email not found in token.");

            var doctor = await _academyContext.Doctors.FirstOrDefaultAsync(d => d.Email == userEmail);
            if (doctor == null)
                return NotFound("Doctor profile not found.");

            // الخطوة الأولى: رجّع الكورسات بتاعة الدكتور (لـ DropDown)
            var availableCourses = await _unitOfWork.Repository<AvailableCourse>()
                .GetAllIncludingAsync(ac => ac.DoctorId == doctor.Id, ac => ac.Course);

            if (availableCourses == null || !availableCourses.Any())
                return NotFound("No courses found for this doctor.");

            // لو ما تمش اختيار كورس أو سنة أو ترم → رجّع الكورسات فقط
            if (!availableCourseId.HasValue || !academicYear.HasValue || !semester.HasValue)
            {
                var courseOptions = availableCourses.Select(ac => new
                {
                    AvailableCourseId = ac.Id,
                    CourseName = ac.Course.Name,
                    AcademicYear = ac.AcademicYears,
                    Semester = ac.Semester
                }).ToList();

                return Ok(new
                {
                    Courses = courseOptions,
                    Message = "Please select a course, academic year, and semester to view students."
                });
            }

            // الخطوة الثانية: نجيب الكورس المتاح بناءً على الاختيار
            var selectedCourse = availableCourses.FirstOrDefault(ac =>
                ac.Id == availableCourseId.Value &&
                ac.AcademicYears == academicYear.Value &&
                ac.Semester == semester.Value
            );

            if (selectedCourse == null)
                return NotFound("Selected course not found for the given academic year and semester.");

            // الخطوة الثالثة: نجيب الطلاب اللي سجلوا الكورس ده
            var students = await _unitOfWork.Repository<Student>()
                .GetAllIncludingAsync(
                    s => s.Courses.Any(c => c.CourseId == selectedCourse.CourseId),
                    s => s.Courses
                );

            var studentData = students.Select(s => new
            {
                StudentId = s.Id,
                s.Name,
                s.Email,
                Scores = s.Courses
                    .Where(c => c.CourseId == selectedCourse.CourseId)
                    .Select(c => new
                    {
                        c.ClassWorkScore,
                        c.PracticalScore,
                        c.FinalScore,
                        c.Grade
                    }).ToList()
            }).ToList();

            return Ok(new
            {
                Course = new
                {
                    selectedCourse.Id,
                    CourseName = selectedCourse.Course.Name,
                    AcademicYear = selectedCourse.AcademicYears,
                    Semester = selectedCourse.Semester
                },
                Students = studentData
            });
        }


        [HttpGet("export-course-students")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> ExportCourseStudentsToExcel([FromQuery] int? availableCourseId, [FromQuery] int? academicYear, [FromQuery] Semster? semester)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("Email not found in token.");

            var doctor = await _academyContext.Doctors.FirstOrDefaultAsync(d => d.Email == userEmail);
            if (doctor == null)
                return NotFound("Doctor profile not found.");

            var availableCourses = await _unitOfWork.Repository<AvailableCourse>()
                .GetAllIncludingAsync(ac => ac.DoctorId == doctor.Id, ac => ac.Course);

            if (!availableCourses.Any())
                return NotFound("No courses found for this doctor.");

            if (!availableCourseId.HasValue || !academicYear.HasValue || !semester.HasValue)
                return BadRequest("Please provide course ID, academic year, and semester.");

            var selectedCourse = availableCourses.FirstOrDefault(ac =>
                ac.Id == availableCourseId.Value &&
                ac.AcademicYears == academicYear.Value &&
                ac.Semester == semester.Value);

            if (selectedCourse == null)
                return NotFound("Selected course not found.");

            // ✅ نجيب الطلاب من علاقة Student.Courses (مش من AssignedCourse)
            var students = await _unitOfWork.Repository<Student>()
                .GetAllIncludingAsync(
                    s => s.Courses.Any(c => c.CourseId == selectedCourse.CourseId),
                    s => s.Courses
                );

            if (!students.Any())
                return NotFound("No students found for this course.");

            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Course Students");

            // Add headers
            worksheet.Cell(1, 1).Value = "Student ID";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "Email";
            worksheet.Cell(1, 4).Value = "ClassWork Score";
            worksheet.Cell(1, 5).Value = "Practical Score";
            worksheet.Cell(1, 6).Value = "Final Score";
            worksheet.Cell(1, 7).Value = "Grade";

            int row = 2;
            foreach (var student in students)
            {
                var courseScore = student.Courses.FirstOrDefault(c => c.CourseId == selectedCourse.CourseId);

                worksheet.Cell(row, 1).Value = student.Id;
                worksheet.Cell(row, 2).Value = student.Name;
                worksheet.Cell(row, 3).Value = student.Email;
                worksheet.Cell(row, 4).Value = courseScore?.ClassWorkScore;
                worksheet.Cell(row, 5).Value = courseScore?.PracticalScore;
                worksheet.Cell(row, 6).Value = courseScore?.FinalScore;
                worksheet.Cell(row, 7).Value = courseScore?.Grade;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            var fileName = $"Course_Students_{selectedCourse.Course.Name}_{academicYear}_{semester}.xlsx";
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost("upload-course-student-grades")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UploadCourseStudentGrades(IFormFile file, [FromQuery] int availableCourseId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("Email not found in token.");

            var doctor = await _academyContext.Doctors.FirstOrDefaultAsync(d => d.Email == userEmail);
            if (doctor == null)
                return NotFound("Doctor not found.");

            var availableCourse = await _academyContext.Availablecourses
                .Include(ac => ac.Course)
                .FirstOrDefaultAsync(ac => ac.Id == availableCourseId && ac.DoctorId == doctor.Id);

            if (availableCourse == null)
                return NotFound("Available course not found for this doctor.");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip headers

            int updatedCount = 0;

            foreach (var row in rows)
            {
                var studentId = row.Cell(1).GetValue<int>();

                var student = await _academyContext.Students
                    .Include(s => s.Courses)
                    .FirstOrDefaultAsync(s => s.Id == studentId);

                if (student == null)
                    continue;

                var courseRecord = student.Courses.FirstOrDefault(c => c.CourseId == availableCourse.CourseId);
                if (courseRecord == null)
                    continue;

                // Try get values safely
                var classWorkScore = row.Cell(4).GetValue<double?>();
                var practicalScore = row.Cell(5).GetValue<double?>();
                var finalScore = row.Cell(6).GetValue<double?>();
                var grade = row.Cell(7).GetValue<string>();

                if (classWorkScore.HasValue)
                    courseRecord.ClassWorkScore = (float)classWorkScore.Value;

                if (practicalScore.HasValue)
                    courseRecord.PracticalScore = (float)practicalScore.Value;

                if (finalScore.HasValue)
                    courseRecord.FinalScore = (float)finalScore.Value;

                if (!string.IsNullOrEmpty(grade))
                    courseRecord.Grade = grade;

                updatedCount++;
            }

            await _academyContext.SaveChangesAsync();

            return Ok($"{updatedCount} student grades updated successfully.");
        }




    }
}
