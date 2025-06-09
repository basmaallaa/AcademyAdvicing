using Academy.Core.Models;
using Academy.Core.ServicesInterfaces;
using Academy.Repo.Data;
using Academy.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademyAdvicingGp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly AcademyContext _academyDbContext;
        private readonly ReportService _reportService;

        public ReportController(AcademyContext academyDbContext, IWebHostEnvironment env, IReportService reportService)
        {
            _academyDbContext = academyDbContext;
            _reportService = (ReportService?)reportService;
        }

        [HttpGet("CandidatesReport")]
        public async Task<IActionResult> GetCandidatesReportAsync()
        {
            // تصفية الطلاب من قاعدة البيانات
            var graduatesData = await _academyDbContext.Students
                .Where(student => student.Level.ToLower() == "four" && student.CompeletedHours >= 117 && student.CompeletedHours < 132)
                .ToListAsync();

            if (!graduatesData.Any())
            {
                return NotFound(new { Message = "No Candidates found with the required hours." });
            }

            // الحصول على آخر ID للتقرير أو تعيين ID جديد
            var latestReportId = (_academyDbContext.Reports.Max(r => (int?)r.Id) ?? 0) + 1;

            // إنشاء التقرير
            var report = await _reportService.GenerateCandidatesReportAsync(latestReportId, graduatesData);

            // حفظ التقرير في قاعدة البيانات
            _academyDbContext.Reports.Add(report);
            await _academyDbContext.SaveChangesAsync();

            // تحقق من وجود الملف
            if (!System.IO.File.Exists(report.FilePath))
            {
                return NotFound(new { Message = "The generated report file was not found." });
            }

            // تحميل الملف كـ PDF
            var fileBytes = System.IO.File.ReadAllBytes(report.FilePath);
            return File(fileBytes, "application/pdf", Path.GetFileName(report.FilePath));
        }


        [HttpGet("graduatesReport")]
        public async Task<IActionResult> GetGraduatesReportAsync()
        {
            // تصفية الطلاب من قاعدة البيانات
            var graduatesData = await _academyDbContext.Students
                .Where(student => student.Level.ToLower() == "four" && student.CompeletedHours >= 132)
                .ToListAsync();

            if (!graduatesData.Any())
            {
                return NotFound(new { Message = "No graduates found with the required hours." });
            }

            // الحصول على آخر ID للتقرير أو تعيين ID جديد
            var latestReportId = (_academyDbContext.Reports.Max(r => (int?)r.Id) ?? 0) + 1;

            // إنشاء التقرير
            var report = await _reportService.GenerateGraduatesReportAsync(latestReportId, graduatesData);

            // حفظ التقرير في قاعدة البيانات
            _academyDbContext.Reports.Add(report);
            await _academyDbContext.SaveChangesAsync();

            // تحقق من وجود الملف
            if (!System.IO.File.Exists(report.FilePath))
            {
                return NotFound(new { Message = "The generated report file was not found." });
            }

            // تحميل الملف كـ PDF
            var fileBytes = System.IO.File.ReadAllBytes(report.FilePath);
            return File(fileBytes, "application/pdf", Path.GetFileName(report.FilePath));
        }


    }
}
