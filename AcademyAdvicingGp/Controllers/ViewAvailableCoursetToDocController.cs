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
        public async Task<IActionResult> GetDoctorCourses(string level)
        {
            // 1. استخراج الإيميل من التوكن
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("Email not found in token.");

            // 2. البحث عن الدكتور بناءً على الإيميل
            var doctor = await _academyContext.Doctors
                .FirstOrDefaultAsync(d => d.Email == userEmail);

            if (doctor == null)
                return NotFound("Doctor profile not found for the current user.");

            // 3. حساب السنة الأكاديمية والترم الحالي
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

            // 4. جلب الكورسات المرتبطة بالدكتور بالسنة، الترم، والليفيل الحاليين
            // محاولة تحويل الـ string إلى enum من نوع Levels
            if (!Enum.TryParse<Levels>(level, true, out var parsedLevel))
                return BadRequest("Invalid level value.");

            // بعد التحويل الناجح، استخدم parsedLevel في المقارنة
            var courses = await _academyContext.Availablecourses
                .Where(ac => ac.DoctorId == doctor.Id &&
                             ac.AcademicYears == academicYear &&
                             ac.Semester == semester &&
                             ac.Level == parsedLevel)
                .Include(ac => ac.Course)
                .ToListAsync();


            if (courses == null || !courses.Any())
                return NotFound("No assigned courses found for this doctor in the current year, semester, and level.");

            // 5. تجهيز النتيجة
            var result = courses.Select(course => new
            {
                AvailableCourseId = course.Id,
                Name = course.Course.Name,
                Code = course.Course.CourseCode,
                AcademicYear = course.AcademicYears,
                Semester = course.Semester,
                Level = course.Level
            });

            return Ok(result);
        }



        [HttpGet("export-course-students")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> ExportCourseStudentsToExcel([FromQuery] int availableCourseId)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("Email not found in token.");

            var doctor = await _academyContext.Doctors.FirstOrDefaultAsync(d => d.Email == userEmail);
            if (doctor == null)
                return NotFound("Doctor profile not found.");

            DateTime now = DateTime.Now;
            int currentYear = now.Year;
            string academicYear = $"{currentYear}/{currentYear + 1}";

            Semster semester;
            if (now.Month >= 9 && now.Month <= 12)
                semester = Semster.Spring;
            else if (now.Month >= 1 && now.Month <= 8)
                semester = Semster.Fall;
            else
                return BadRequest("No available courses in summer (June–August).");

            var availableCourses = await _unitOfWork.Repository<AvailableCourse>()
                .GetAllIncludingAsync(ac => ac.DoctorId == doctor.Id, ac => ac.Course);

            if (!availableCourses.Any())
                return NotFound("No courses found for this doctor.");

            var selectedCourse = availableCourses.FirstOrDefault(ac =>
                ac.Id == availableCourseId &&
                ac.AcademicYears == academicYear &&
                ac.Semester == semester);

            if (selectedCourse == null)
                return NotFound("Selected course not found for the current semester and academic year.");

            var students = await _unitOfWork.Repository<Student>()
                .GetAllIncludingAsync(
                    s => s.Courses.Any(c => c.CourseId == selectedCourse.CourseId),
                    s => s.Courses
                );

            if (!students.Any())
                return NotFound("No students found for this course.");

            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Course Students");

            worksheet.Cell(1, 1).Value = "Student ID";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "Email";
            worksheet.Cell(1, 4).Value = "ClassWork Score";
            worksheet.Cell(1, 5).Value = "Practical Score";
            worksheet.Cell(1, 6).Value = "Final Score";
            worksheet.Cell(1, 7).Value = "Grade";
            worksheet.Cell(1, 8).Value = "Total Grades";

            int row = 2;
            foreach (var student in students)
            {
                var courseScore = student.Courses.FirstOrDefault(c => c.CourseId == selectedCourse.CourseId);

                worksheet.Cell(row, 1).Value = student.Id;
                worksheet.Cell(row, 2).Value = student.Name;
                worksheet.Cell(row, 3).Value = student.Email;
                worksheet.Cell(row, 4).Value = courseScore?.ClassWorkScore ?? 0;
                worksheet.Cell(row, 5).Value = courseScore?.PracticalScore ?? 0;
                worksheet.Cell(row, 6).Value = courseScore?.FinalScore ?? 0;
                worksheet.Cell(row, 7).Value = courseScore?.Grade ?? "null";
                worksheet.Cell(row, 8).Value = courseScore?.TotalGrades ?? 0;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            var fileName = $"Course_Students_{selectedCourse.Course.Name}{academicYear.Replace("/", "-")}{semester}.xlsx";
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
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header row

            int updatedCount = 0;

            foreach (var row in rows)
            {
                var studentIdCellValue = row.Cell(1).GetValue<string>()?.Trim();
                if (!int.TryParse(studentIdCellValue, out var studentId))
                    continue;

                var student = await _academyContext.Students
                    .Include(s => s.Courses)
                    .FirstOrDefaultAsync(s => s.Id == studentId);

                if (student == null)
                    continue;

                var courseRecord = student.Courses.FirstOrDefault(c => c.CourseId == availableCourse.CourseId);
                if (courseRecord == null)
                    continue;

                var classWorkScoreStr = row.Cell(4).GetValue<string>()?.Trim();
                var practicalScoreStr = row.Cell(5).GetValue<string>()?.Trim();
                var finalScoreStr = row.Cell(6).GetValue<string>()?.Trim();

                courseRecord.ClassWorkScore = TryParseNullableFloat(classWorkScoreStr);
                _academyContext.Entry(courseRecord).Property(c => c.ClassWorkScore).IsModified = true;

                courseRecord.PracticalScore = TryParseNullableFloat(practicalScoreStr);
                _academyContext.Entry(courseRecord).Property(c => c.PracticalScore).IsModified = true;

                courseRecord.FinalScore = TryParseNullableFloat(finalScoreStr);
                _academyContext.Entry(courseRecord).Property(c => c.FinalScore).IsModified = true;

                // حساب TotalGrades = ClassWorkScore + PracticalScore + FinalScore
                float totalGrades = 0f;
                if (courseRecord.ClassWorkScore.HasValue)
                    totalGrades += courseRecord.ClassWorkScore.Value;
                if (courseRecord.PracticalScore.HasValue)
                    totalGrades += courseRecord.PracticalScore.Value;
                if (courseRecord.FinalScore.HasValue)
                    totalGrades += courseRecord.FinalScore.Value;

                courseRecord.TotalGrades = totalGrades;
                _academyContext.Entry(courseRecord).Property(c => c.TotalGrades).IsModified = true;

                // حساب التقدير بناء على TotalGrades المحسوبة
                courseRecord.Grade = CalculateGrade(totalGrades);
                _academyContext.Entry(courseRecord).Property(c => c.Grade).IsModified = true;

                updatedCount++;
            }

            await _academyContext.SaveChangesAsync();

            return Ok($"{updatedCount} student grades updated successfully.");
        }

        private float? TryParseNullableFloat(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            if (float.TryParse(input, out var result))
                return result;

            return null;
        }

        private string CalculateGrade(float totalGrade)
        {
            if (totalGrade >= 90) return "A+";
            if (totalGrade >= 85) return "A";
            if (totalGrade >= 80) return "B+";
            if (totalGrade >= 75) return "B";
            if (totalGrade >= 70) return "C+";
            if (totalGrade >= 65) return "C";
            if (totalGrade >= 60) return "D+";
            if (totalGrade >= 50) return "D";
            return "F";
        }







    }
}