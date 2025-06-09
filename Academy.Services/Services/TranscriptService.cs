using Academy.Core.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Elements;

namespace Academy.Services.Services
{
    public class TranscriptService
    {
        public byte[] GenerateTranscriptPdf(TranscriptDto transcript, byte[]? studentImageBytes, byte[]? universityLogoBytes)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(10);
                    page.DefaultTextStyle(x => x.FontSize(7));

                    // Header
                    page.Header().Row(headerRow =>
                    {
                        headerRow.ConstantColumn(100).Height(80).Element(x =>
                        {
                            var image = universityLogoBytes ?? PlaceholderImage(100, 80);
                            x.Image(image).FitArea();
                        });

                        headerRow.RelativeColumn().Column(headerCol =>
                        {
                            headerCol.Item().AlignCenter().Text("جامعة حلوان").FontSize(14).Bold();
                            headerCol.Item().AlignCenter().Text("HELWAN UNIVERSITY").FontSize(10).Bold();
                            headerCol.Item().AlignCenter().Text("كلية الحاسبات والذكاء الاصطناعي").FontSize(10);
                            headerCol.Item().AlignCenter().Text("Faculty of Computers and Artificial Intelligence").FontSize(9);
                            headerCol.Item().AlignCenter().Text("Student Transcript").FontSize(14).Bold().Underline();
                            headerCol.Item().AlignCenter().Text($"Student Name: {transcript.StudentName}").Bold().FontSize(13);
                        });

                        headerRow.ConstantColumn(100).Height(80).Element(x =>
                        {
                            var image = studentImageBytes ?? PlaceholderImage(100, 80);
                            x.Image(image).FitArea();
                        });
                    });

                    // Body
                    page.Content().Padding(5).ScaleToFit().Column(col =>
                    {
                        // Summary Table
                        col.Item().Row(row =>
                        {
                            row.RelativeColumn();
                            row.ConstantColumn(220).AlignRight().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(100);
                                    columns.RelativeColumn();
                                });

                                table.Cell().Row(1).Column(1).Element(CellStyleHeader).Text("Total GPA");
                                table.Cell().Row(1).Column(2).Element(CellStyleHeader).Text(transcript.GPA.ToString("0.00"));

                                table.Cell().Row(2).Column(1).Element(CellStyleHeader).Text("Grade");
                                table.Cell().Row(2).Column(2).Element(CellStyleHeader).Text(GetGradeLabel(transcript.GPA));

                                table.Cell().Row(3).Column(1).Element(CellStyleHeader).Text("Hours");
                                table.Cell().Row(3).Column(2).Element(CellStyleHeader).Text(transcript.CompletedHours.ToString());

                                table.Cell().Row(4).Column(1).Element(CellStyleHeader).Text("State");
                                table.Cell().Row(4).Column(2).Element(CellStyleHeader).Text(transcript.Status);

                                table.Cell().Row(5).Column(1).Element(CellStyleHeader).Text("Level");
                                table.Cell().Row(5).Column(2).Element(CellStyleHeader).Text(transcript.Levels.ToString() ?? "--");

                                static IContainer CellStyleHeader(IContainer container) =>
                                    container.Border(1).BorderColor(Colors.Grey.Medium).Padding(2).AlignCenter();
                            });
                        });

                        // Courses layout
                        col.Item().Row(row =>
                        {
                            row.Spacing(10);

                            row.RelativeColumn().Element(x =>
                                AddSection(x, "University Requirements",
                                    transcript.Courses.Where(c => c.Category == Core.Enums.courseCategory.UniversityCourse).ToList(),
                                    "12 CH: 6 Compulsory + 6 Electives"));

                            row.RelativeColumn().Element(x =>
                                AddSection(x, "Faculty Requirements",
                                    transcript.Courses.Where(c => c.Category == Core.Enums.courseCategory.FacultyCourse).ToList(),
                                    "54 CH: 45 Compulsory + 9 Electives"));
                        });

                        col.Item().Element(x =>
                            AddSection(x, "Specialization Requirements",
                                transcript.Courses.Where(c => c.Category == Core.Enums.courseCategory.SpecializationCourse).ToList(),
                                "57 CH: 42 Compulsory + 15 Electives"));

                    });

                    // Footer
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Generated on: ").SemiBold();
                        text.Span($"{DateTime.Now:d}");
                    });
                });
            }).GeneratePdf();
        }

        private void AddSection(IContainer container, string title, List<CourseGradeDto> courses, string? subtitle = null)
        {
            container.Column(col =>
            {
                col.Item().PaddingTop(4).Text(title).FontSize(10).Bold().Underline();

                if (!string.IsNullOrEmpty(subtitle))
                {
                    col.Item().Text(subtitle).FontSize(10).Bold();
                }

                col.Item().PaddingTop(2).Element(x =>
                {
                    if (courses == null || !courses.Any())
                    {
                        x.Text("No courses available.").Italic().FontSize(7);
                        return;
                    }

                    x.Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(35);  // Code
                            columns.RelativeColumn(0.7f); // Name
                            columns.ConstantColumn(40);  // Hours
                            columns.ConstantColumn(40);  // Grade
                            columns.ConstantColumn(40);  // Total Grade
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyleHeader).Text("Code").Bold().FontSize(7);
                            header.Cell().Element(CellStyleHeader).Text("Course Name").Bold().FontSize(7);
                            header.Cell().Element(CellStyleHeader).AlignCenter().Text("Hours").Bold().FontSize(7);
                            header.Cell().Element(CellStyleHeader).AlignCenter().Text("Grade").Bold().FontSize(7);
                            header.Cell().Element(CellStyleHeader).AlignCenter().Text("Total Grade").Bold().FontSize(7);
                        });

                        foreach (var course in courses)
                        {
                            table.Cell().Element(CellStyle).Text(course.Code).FontSize(7);
                            table.Cell().Element(CellStyle).Text(course.CourseName).FontSize(7);
                            table.Cell().Element(CellStyle).AlignCenter().Text(course.Hours.ToString()).FontSize(7);
                            table.Cell().Element(CellStyle).AlignCenter().Text(course.GradeLetter).FontSize(7);
                            table.Cell().Element(CellStyle).AlignCenter().Text(course.TotalGrades.HasValue ? course.TotalGrades.Value.ToString("0.00") : "--").FontSize(7);
                        }

                        IContainer CellStyle(IContainer container) =>
                            container.Border(1).BorderColor(Colors.Grey.Darken1).Padding(2);

                        IContainer CellStyleHeader(IContainer container) =>
                            container.Background(Colors.Grey.Lighten2)
                                     .Border(1).BorderColor(Colors.Grey.Darken1).Padding(2).AlignCenter();
                    });
                });
            });
        }


        private string GetGradeLabel(double gpa)
        {
            if (gpa >= 3.4) return "Excellent";
            if (gpa >= 2.8) return "Very Good";
            if (gpa >= 2.3) return "Good";
            if (gpa >= 1.9) return "Pass";
            if (gpa >= 1.4) return "Weak";
            return "fail";
        }

        private byte[] PlaceholderImage(int width, int height)
        {
            return Placeholders.Image(width, height);
        }
    }
}
