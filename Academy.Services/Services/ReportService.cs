using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using Academy.Core.Models;
using Academy.Core.ServicesInterfaces;
using Academy.Repo.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Document = QuestPDF.Fluent.Document;

namespace Academy.Services.Services
{
    public class ReportService :IReportService
    {
        private readonly IWebHostEnvironment _env;
        private readonly AcademyContext _academyDbContext;
        private IWebHostEnvironment env;
        public ReportService(IWebHostEnvironment env, AcademyContext academyDbContext)
        {
            _env = env;
            _academyDbContext = academyDbContext;
        }

        public async Task<Report> GenerateGraduatesReportAsync(int reportId, List<Student> data)
        {
            var fileName = $"Report_{reportId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var reportsDir = Path.Combine(_env.WebRootPath ?? "wwwroot", "reports");

            if (!Directory.Exists(reportsDir))
                Directory.CreateDirectory(reportsDir);

            var filePath = Path.Combine(reportsDir, fileName);

            // تحديد العام الدراسي
            int currentYear = DateTime.Now.Year;
            int nextYear = currentYear + 1;
            int previousYear = currentYear - 1;

            string academicYear;
            if (DateTime.Now.Month >= 10) // من أكتوبر إلى ديسمبر
                academicYear = $"  {nextYear} /  {currentYear}";
            else // من يناير إلى سبتمبر
                academicYear = $"  {currentYear} /  {previousYear}";

            // تحديد الفصل الدراسي
            string semester;
            int currentMonth = DateTime.Now.Month;
            if (currentMonth >= 10 || currentMonth < 2)
                semester = "الأول";
            else if (currentMonth >= 2 && currentMonth <= 6)
                semester = "الثاني";
            else
                semester = "(الثالث(الصيفي";

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));
                    page.Content().Column(column =>
                    {
                        // Header Section
                        column.Item().Column(innerColumn =>
                        {
                            innerColumn.Item().Element(imageContainer =>
                            {
                                var imagePath = Path.Combine("wwwroot", "Images", "Logo1.jpg");
                                byte[] imageBytes = File.ReadAllBytes(imagePath);

                                imageContainer
                                    .AlignRight()
                                    .Height(100)
                                    .Width(100)
                                    .Image(imageBytes, ImageScaling.FitWidth);
                            });

                            innerColumn.Item().Text(" كلية الحاسبات والذكاء الاصطناعي")
                                .AlignRight()
                                .FontSize(16)
                                .Bold()
                                .ParagraphSpacing(2);
                        });

                        column.Item().Text("");

                        // Semester and Department Information
                        column.Item().Row(row =>
                        {
                            row.RelativeColumn(1).Text($"{academicYear} : العام الدراسي ").AlignCenter().FontSize(14);
                        });

                        column.Item().Row(row =>
                        {
                            row.Spacing(2);
                            row.RelativeColumn(1).Text($"الفصل الدراسي: {semester}").AlignLeft().FontSize(14);
                        });

                        column.Item().Row(row =>
                        {
                            row.RelativeColumn(1).Text("القسم: هندسة البرمجيات").AlignLeft().FontSize(14);
                        });

                        column.Item().Text("");

                        // Title Section
                        column.Item().AlignCenter().Text("بيان بتقديرات الطلبة الخريجين")
                            .FontSize(16).Bold().Underline();

                        column.Item().Text("");

                        // Table with Borders
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);   // التقدير
                                columns.RelativeColumn(1);   // النسبه
                                columns.RelativeColumn(1);   // التراكمي
                                columns.RelativeColumn(1);   // الساعات الفعلية
                                columns.RelativeColumn(0.8f); // المستوى
                                columns.RelativeColumn(2);   // اسم الطالب
                                columns.RelativeColumn(1.5f); // رقم الطالب
                                columns.RelativeColumn(0.5f); // م
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#000000").Text("التقدير").FontColor("#FFFFFF").AlignCenter();
                                header.Cell().Background("#000000").Text("النسبة").FontColor("#FFFFFF").AlignCenter();
                                header.Cell().Background("#000000").Text("التراكمي").FontColor("#FFFFFF").AlignCenter();
                                header.Cell().Background("#000000").Text("الساعات الفعلية").FontColor("#FFFFFF").AlignCenter();
                                header.Cell().Background("#000000").Text("المستوى").FontColor("#FFFFFF").AlignCenter();
                                header.Cell().Background("#000000").Text("اسم الطالب").FontColor("#FFFFFF").AlignCenter();
                                header.Cell().Background("#000000").Text("رقم الطالب").FontColor("#FFFFFF").AlignCenter();
                                header.Cell().Background("#000000").Text("م").FontColor("#FFFFFF").AlignCenter();
                            });

                            int index = 1; // متغير رقم التسلسل
                            foreach (var student in data)
                            {
                                var percentage = (student.GPA * 100) / 4.00; // حساب النسبة
                                string grade;

                                if (student.GPA > 3.4)
                                    grade = "امتياز";
                                else if (student.GPA > 2.8)
                                    grade = "جيد جدًا";
                                else if (student.GPA > 2.4)
                                    grade = "جيد";
                                else if (student.GPA >= 2.0)
                                    grade = "مقبول";
                                else
                                    grade = "راسب";

                                table.Cell().Border(1).Text(grade).AlignCenter();
                                table.Cell().Border(1).Text($"{percentage:F2}%").AlignCenter();
                                table.Cell().Border(1).Text($"{student.GPA:F2}").AlignCenter();
                                table.Cell().Border(1).Text(student.CompeletedHours.ToString()).AlignCenter();
                                table.Cell().Border(1).Text("4").AlignCenter();
                                table.Cell().Border(1).Text(student.Name).AlignCenter();
                                table.Cell().Border(1).Text(student.UserName).AlignCenter();
                                table.Cell().Border(1).Text(index.ToString()).AlignCenter(); // رقم التسلسل

                                index++; // زيادة رقم التسلسل
                            }
                        });
                    });
                });
            }).GeneratePdf(filePath);

            var report = new Report
            {
                Type = "Graduates Report",
                FilePath = filePath
            };

            return report;
        }




        public async Task<Report> GenerateCandidatesReportAsync(int reportId, List<Student> data)
        {
            var fileName = $"Report_{reportId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var reportsDir = Path.Combine(_env.WebRootPath ?? "wwwroot", "reports");

            if (!Directory.Exists(reportsDir))
                Directory.CreateDirectory(reportsDir);

            var filePath = Path.Combine(reportsDir, fileName);

            // تحديد العام الدراسي
            int currentYear = DateTime.Now.Year;
            int nextYear = currentYear + 1;
            int previousYear = currentYear - 1;

            string academicYear;
            if (DateTime.Now.Month >= 10) // من أكتوبر إلى ديسمبر
                academicYear = $" {nextYear}/ {currentYear}";
            else // من يناير إلى سبتمبر
                academicYear = $" {currentYear}/ {previousYear}";

            // تحديد الفصل الدراسي
            string semester;
            int currentMonth = DateTime.Now.Month;
            if (currentMonth >= 10 || currentMonth < 2)
                semester = "الأول";
            else if (currentMonth >= 2 && currentMonth <= 6)
                semester = "الثاني";
            else
                semester = "(الثالث(الصيفي";

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));
                    page.Content().Column(column =>
                    {
                        // Header Section
                        column.Item().Column(innerColumn =>
                        {
                            innerColumn.Item().Element(imageContainer =>
                            {
                                var imagePath = Path.Combine("wwwroot", "Images", "Logo1.jpg");
                                byte[] imageBytes = File.ReadAllBytes(imagePath);

                                imageContainer
                                    .AlignRight()
                                    .Height(100)
                                    .Width(100)
                                    .Image(imageBytes, ImageScaling.FitWidth);
                            });

                            innerColumn.Item().Text(" كلية الحاسبات والذكاء الاصطناعي")
                                .AlignRight()
                                .FontSize(16)
                                .Bold()
                                .ParagraphSpacing(5);
                        });

                        column.Item().Text("");

                        // Semester and Department Information
                        column.Item().Row(row =>
                        {
                            row.RelativeColumn(1).Text($"{academicYear} : العام الدراسي ").AlignCenter().FontSize(14);
                        });

                        column.Item().Row(row =>
                        {
                            row.Spacing(2);
                            row.RelativeColumn(1).Text($"الفصل الدراسي: {semester}").AlignLeft().FontSize(14);
                        });

                        column.Item().Row(row =>
                        {
                            row.RelativeColumn(1).Text("القسم: هندسة البرمجيات").AlignLeft().FontSize(14);
                        });

                        column.Item().Text("");

                        // Title Section
                        column.Item().AlignCenter().Text("بيان بتقديرات الطلبة المؤهلين للتخرج")
                            .FontSize(16).Bold().Underline();

                        column.Item().Text("");

                        // Table with Borders
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);   // التقدير
                                columns.RelativeColumn(1);   // النسبة
                                columns.RelativeColumn(1);   // التراكمي
                                columns.RelativeColumn(1);   // الساعات الفعلية
                                columns.RelativeColumn(0.8f); // المستوى
                                columns.RelativeColumn(2);   // اسم الطالب
                                columns.RelativeColumn(1.5f); // رقم الطالب
                                columns.RelativeColumn(0.5f); // م
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#000000").Text("التقدير").FontColor("#FFFFFF").AlignCenter();
                                header.Cell().Background("#000000").Text("النسبة").FontColor("#FFFFFF").AlignCenter();
                                header.Cell().Background("#000000").Text("التراكمي").FontColor("#FFFFFF").AlignCenter();
                                header.Cell().Background("#000000").Text("الساعات الفعلية").FontColor("#FFFFFF").AlignCenter();
                                header.Cell().Background("#000000").Text("المستوى").FontColor("#FFFFFF").AlignCenter();
                                header.Cell().Background("#000000").Text("اسم الطالب").FontColor("#FFFFFF").AlignCenter();
                                header.Cell().Background("#000000").Text("رقم الطالب").FontColor("#FFFFFF").AlignCenter();
                                header.Cell().Background("#000000").Text("م").FontColor("#FFFFFF").AlignCenter();
                            });

                            int index = 1; // متغير رقم التسلسل
                            foreach (var student in data)
                            {
                                var percentage = (student.GPA * 100) / 4.00; // حساب النسبة
                                string grade;

                                if (student.GPA > 3.4)
                                    grade = "امتياز";
                                else if (student.GPA > 2.8)
                                    grade = "جيد جدًا";
                                else if (student.GPA > 2.4)
                                    grade = "جيد";
                                else if (student.GPA >= 2.0)
                                    grade = "مقبول";
                                else
                                    grade = "راسب";

                                table.Cell().Border(1).Text(grade).AlignCenter();
                                table.Cell().Border(1).Text($"{percentage:F2}%").AlignCenter();
                                table.Cell().Border(1).Text($"{student.GPA:F2}").AlignCenter();
                                table.Cell().Border(1).Text(student.CompeletedHours.ToString()).AlignCenter();
                                table.Cell().Border(1).Text("4").AlignCenter();
                                table.Cell().Border(1).Text(student.Name).AlignCenter();
                                table.Cell().Border(1).Text(student.UserName).AlignCenter();
                                table.Cell().Border(1).Text(index.ToString()).AlignCenter(); // رقم التسلسل

                                index++; // زيادة رقم التسلسل
                            }
                        });
                    });
                });
            }).GeneratePdf(filePath);

            var report = new Report
            {
                Type = "Candidates Report",
                FilePath = filePath
            };

            return report;
        }
    }
}
