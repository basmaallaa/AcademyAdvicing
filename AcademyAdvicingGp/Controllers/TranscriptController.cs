using Academy.Core.Dtos;
using Academy.Core.Models;
using Academy.Core.Models.Identity;
using Academy.Repo.Data;
using Academy.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcademyAdvicingGp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranscriptController : ControllerBase
    {
        private readonly AcademyContext _context;
        private readonly TranscriptService _transcriptService;
        private readonly UserManager<AppUser> _userManager;

        public TranscriptController(
            AcademyContext context,
            TranscriptService transcriptService,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _transcriptService = transcriptService;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet("transcript")]
        public async Task<IActionResult> DownloadTranscriptForCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return NotFound();

            var student = _context.Students
                .Include(s => s.Courses)
                    .ThenInclude(ac => ac.Course)
                .FirstOrDefault(s => s.Email == email);

            if (student == null)
                return NotFound();

            var allCourses = _context.Courses.ToList();
            var assignedCourses = student.Courses;

            var transcriptCourses = allCourses.Select(course =>
            {
                var taken = assignedCourses.FirstOrDefault(ac => ac.CourseId == course.CourseId);

                return new CourseGradeDto
                {
                    Code = course.CourseCode,
                    CourseName = course.Name,
                    Hours = course.CreditHours,
                    Category = course.category,
                    GradeLetter = taken?.Grade ?? "--",
                    TotalGrades = taken?.TotalGrades
                };
            }).ToList();

            var dto = new TranscriptDto
            {
                StudentName = student.Name,
                GPA = student.GPA,
                CompletedHours = student.CompeletedHours,
                Status = student.Status.ToString(),
                Levels = student.Level,
                Courses = transcriptCourses
            };

            // Load images (logo and student image)

            //var logoBytes = await System.IO.File.ReadAllBytesAsync("wwwroot/images/helwan-logo.png");
            //var studentImageBytes = student.ImagePath != null && System.IO.File.Exists(student.ImagePath)
            //    ? await System.IO.File.ReadAllBytesAsync(student.ImagePath)
            //    : null;
            var logoBytes = await System.IO.File.ReadAllBytesAsync("wwwroot/images/helwan-logo.png");

            var imageFullPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", student.ImagePath ?? "");
            var studentImageBytes = System.IO.File.Exists(imageFullPath)
                ? await System.IO.File.ReadAllBytesAsync(imageFullPath)
                : null;



            var pdf = _transcriptService.GenerateTranscriptPdf(dto, studentImageBytes, logoBytes);

            return File(pdf, "application/pdf", "Transcript.pdf");
        }
    }
}
