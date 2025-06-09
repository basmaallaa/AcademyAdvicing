using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using Academy.Core.Dtos;
using Academy.Core.Dtos.ScheduleDtos;

namespace Academy.Services.Services
{
    public class SchedulePdfService
    {
		public byte[] GenerateSchedulePdf(List<ScheduleTimeTableDto> schedule)
		{
			var document = Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Size(PageSizes.A4);
					page.Margin(30);
					page.PageColor(Colors.White);
					page.DefaultTextStyle(x => x.FontSize(12));

					page.Header()
						.Text("Student Schedule")
						.SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

					page.Content()
						.Table(table =>
						{
							// Define columns
							table.ColumnsDefinition(columns =>
							{
								columns.RelativeColumn(1); // Day
								columns.RelativeColumn(1); // Start
								columns.RelativeColumn(1); // End
								columns.RelativeColumn(1); // Location
								columns.RelativeColumn(1); // Doctor
								columns.RelativeColumn(1); // Course
							});

							// Header
							table.Header(header =>
							{
								header.Cell().Element(CellStyle).Text("Day");
								header.Cell().Element(CellStyle).Text("Start");
								header.Cell().Element(CellStyle).Text("End");
								header.Cell().Element(CellStyle).Text("Location");
								header.Cell().Element(CellStyle).Text("Doctor");
								header.Cell().Element(CellStyle).Text("Course");

								static IContainer CellStyle(IContainer container)
								{
									return container
										.DefaultTextStyle(x => x.SemiBold())
										.PaddingVertical(5)
										.BorderBottom(1)
										.BorderColor(Colors.Grey.Medium);
								}
							});

							// Rows
							foreach (var item in schedule)
							{
								table.Cell().Text(item.DayOfWeek);
								table.Cell().Text(item.StartTime.ToString("hh\\:mm"));
								table.Cell().Text(item.EndTime.ToString("hh\\:mm"));
								table.Cell().Text(item.Location);
								table.Cell().Text(item.DoctorName);
								table.Cell().Text(item.CourseName);
							}
						});
				});
			});

			using var stream = new MemoryStream();
			document.GeneratePdf(stream);
			return stream.ToArray();
		}

	}
}
