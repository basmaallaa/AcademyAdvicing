using Academy.Core;
using Academy.Core.Dtos.FinalTimeTableDtos;
using Academy.Core.Dtos.ScheduleDtos;
using Academy.Core.Models;
using Academy.Core.ServicesInterfaces;
using Academy.Repo.Data;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;
using QuestPDF.Previewer;

namespace Academy.Services.Services
{
	public class FinalExamTimeTableService : IFinalExamTimeTableService
	{
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
		private readonly AcademyContext _academyContext;

		public FinalExamTimeTableService(IMapper mapper, IUnitOfWork unitOfWork, AcademyContext academyContext)
        {
			_mapper = mapper;
			_unitOfWork = unitOfWork;
			_academyContext = academyContext;
		}
        public async Task<IEnumerable<FinalExamTimeTableDto>> GetAllAsync()
		{
			var FinalTimeTablesq = await _unitOfWork.Repository<FinalExamTimeTable>().GetAllAsync();

			var finalTimeTables = await FinalTimeTablesq
			   .Include(st => st.UploadedBy)
			   .Include(st => st.AvailableCourses)
					.ThenInclude(ac => ac.Course)
			   .ToListAsync();

			return _mapper.Map<IEnumerable<FinalExamTimeTableDto>>(finalTimeTables);
		}

		public async Task<FinalExamTimeTableDto> GetByIdAsync(int id)
		{
			var FinalTimeTabless = await _unitOfWork.Repository<FinalExamTimeTable>().GetAllAsync();

			var finalTimeTable = await FinalTimeTabless
			.Include(st => st.UploadedBy)
			.Include(st => st.AvailableCourses)
				.ThenInclude(ac => ac.Course)
			.FirstOrDefaultAsync(st => st.Id == id);

			return _mapper.Map<FinalExamTimeTableDto>(finalTimeTable);
		}

		public async Task<FinalExamTimeTableDto> AddAsync(CreateFinalExamTimeTableDto dto, int coordinatorId)
		{
			// Validate time range
			if (dto.StartTime == dto.EndTime)
			{
				throw new ArgumentException("Start time and end time cannot be the same.");
			}

			if (dto.StartTime > dto.EndTime)
			{
				throw new ArgumentException("Start time cannot be after end time.");
			}

			// Check if there's already Exam at the same time and location
			var allFinals = await _unitOfWork.Repository<FinalExamTimeTable>().GetAllAsync();
			var conflict = allFinals.Any(s =>
				s.DayOfWeek == dto.DayOfWeek &&
				s.StartTime == dto.StartTime &&
				s.EndTime == dto.EndTime &&
				s.Location.ToLower() == dto.Location.ToLower());

			if (conflict)
			{
				throw new ArgumentException("There is already a Exam scheduled at this time and location.");


			}

			var availableCourse = await _unitOfWork.Repository<AvailableCourse>().GetAsync(dto.AvailableCourseId);
			if (availableCourse == null) return null;  // AvailableCourse not found

			var uploadedBy = await _unitOfWork.Repository<Coordinator>().GetAsync(coordinatorId);
			if (uploadedBy == null) return null;  // User (UploadedBy) not found

			// Map DTO to entity
			var finalTimeTable = _mapper.Map<FinalExamTimeTable>(dto);
			finalTimeTable.AvailableCourses = availableCourse;
			finalTimeTable.UploadedBy = uploadedBy;

			// Add entity to repository
			await _unitOfWork.Repository<FinalExamTimeTable>().AddAsync(finalTimeTable);
			await _unitOfWork.CompleteAsync();

			var fttResult = _mapper.Map<FinalExamTimeTableDto>(finalTimeTable);
			
			return _mapper.Map<FinalExamTimeTableDto>(finalTimeTable);
		}
		

		public async Task UpdateFinalTimeTableAsync(int id, EditFinalExamTimeTableDto dto, int coordinatorId)
		{
			var existingFinal = await _unitOfWork.Repository<FinalExamTimeTable>().GetAsync(id);

			if (existingFinal == null)
				throw new ArgumentException("Final Time Table not found.");

			if (dto.StartTime == dto.EndTime)
			{
				throw new ArgumentException("Start time and end time cannot be the same.");
			}

			if (dto.StartTime > dto.EndTime)
			{
				throw new ArgumentException("Start time must be before end time.");
			}



			// Check for conflict with other time tables (excluding the current one)
			var allFinals = await _unitOfWork.Repository<FinalExamTimeTable>().GetAllAsync();
			var conflict = allFinals.Any(s =>
				s.Id != id &&
				s.DayOfWeek == dto.DayOfWeek &&
				s.StartTime == dto.StartTime &&
				s.EndTime == dto.EndTime &&
				s.Location.ToLower() == dto.Location.ToLower()
			);

			if (conflict)
				throw new ArgumentException("There is already a Final Time Table scheduled at this time and location.");

			// Update values
			//existingSchedule.AvailableCourseId = dto.AvailableCourseId;
			existingFinal.DayOfWeek = dto.DayOfWeek;
			existingFinal.StartTime = dto.StartTime;
			existingFinal.EndTime = dto.EndTime;
			existingFinal.Location = dto.Location;
			existingFinal.UploadedById = coordinatorId;

			_unitOfWork.Repository<FinalExamTimeTable>().Update(existingFinal);
			await _unitOfWork.CompleteAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var finalFimeTable = await _unitOfWork.Repository<FinalExamTimeTable>().GetAsync(id);
			if (finalFimeTable == null) throw new KeyNotFoundException($"Final Time Table with ID {id} not found.");

			_unitOfWork.Repository<FinalExamTimeTable>().Delete(finalFimeTable);
			await _unitOfWork.CompleteAsync();
		}

		public async Task AddBulkFinalTimeTableAsync(CreateFullFinalTimeTableDto dto, int coordinatorId)
		{
			foreach (var finalDto in dto.Courses)
			{
				// check Time Logic
				if (finalDto.StartTime == finalDto.EndTime)
				{
					throw new Exception($"Start time and end time cannot be the same for course at {finalDto.DayOfWeek} in {finalDto.Location}.");
				}

				if (finalDto.StartTime > finalDto.EndTime)
				{
					throw new Exception($"Start time must be before end time for course at {finalDto.DayOfWeek} in {finalDto.Location}.");
				}

				// Check for time aw location conflicts
				var conflict = await _unitOfWork.Repository<FinalExamTimeTable>().GetAllAsync();
				var isConflict = conflict.Any(s =>
					s.DayOfWeek == finalDto.DayOfWeek &&
					s.StartTime == finalDto.StartTime &&
					s.EndTime == finalDto.EndTime &&
					s.Location.ToLower() == finalDto.Location.ToLower()
				);

				if (isConflict)
				{
					throw new Exception($"Conflict at {finalDto.DayOfWeek} {finalDto.StartTime}-{finalDto.EndTime} in {finalDto.Location}");
				}

				var finalExamTimeTable = new FinalExamTimeTable
				{
					AvailableCourseId = finalDto.AvailableCourseId,
					UploadedById = coordinatorId,
					DayOfWeek = finalDto.DayOfWeek,
					StartTime = finalDto.StartTime,
					EndTime = finalDto.EndTime,
					Location = finalDto.Location
				};

				await _unitOfWork.Repository<FinalExamTimeTable>().AddAsync(finalExamTimeTable);
			}

			await _unitOfWork.CompleteAsync();
		}


		public async Task<List<FinalExamTimeTableDto>> GetStudentFinalExamSchedule(int studentId)
		{
			// Step 1: Get student's assigned courses
			var student = await _unitOfWork.Repository<Student>().FirstOrDefaultAsync(s => s.Id == studentId);
			if (student == null)
				return null;

			var assignedCourses = await _unitOfWork.Repository<AssignedCourse>().GetAllAsync(ac => ac.StudentId == studentId);

			var courseIds = assignedCourses.Select(ac => ac.CourseId).Distinct().ToList();

			if (!courseIds.Any())
				return new List<FinalExamTimeTableDto>();

			// Step 2: Get available courses related to the student's assigned courseIds
			var availableCourses = await _unitOfWork.Repository<AvailableCourse>().GetAllAsync(ac => courseIds.Contains(ac.CourseId));

			var availableCourseIds = availableCourses.Select(ac => ac.Id).Distinct().ToList();

			if (!availableCourseIds.Any())
				return new List<FinalExamTimeTableDto>();

			// Step 3: Get schedule entries for those available courses
			var finalTimeTables = await _academyContext.FinalExamTimeTable
			.Where(st => availableCourseIds.Contains(st.AvailableCourseId ?? 0))
			.Include(st => st.UploadedBy)
			.Include(st => st.AvailableCourses)
				.ThenInclude(ac => ac.Course)
			.ToListAsync();

			return _mapper.Map<List<FinalExamTimeTableDto>>(finalTimeTables);
		}


		public byte[] GenerateFinalExamPdf(List<FinalExamTimeTableDto> exams, string studentName, string level)
		{
			QuestPDF.Settings.License = LicenseType.Community;

			var document = Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Margin(30);
					page.Size(PageSizes.A4.Landscape()); // Landscape mode
					page.PageColor(Colors.White);
					page.DefaultTextStyle(x => x.FontSize(12));

					page.Header().Column(col =>
					{
						col.Item().Text($"Student Name: {studentName}").Bold().FontSize(14);
						col.Item().Text($"Level: {level}").FontSize(12);
						col.Item().PaddingVertical(10).Text("Final Exam Schedule")
							.FontSize(20).Bold().FontColor(Colors.Red.Medium);
					});

					page.Content().Table(table =>
					{
						// Define column widths
						table.ColumnsDefinition(columns =>
						{
							columns.RelativeColumn(2); // Day
							columns.RelativeColumn(3); // Date
							columns.RelativeColumn(6); // Course Name (wider)
							columns.RelativeColumn(3); // Location
							columns.RelativeColumn(2); // From
							columns.RelativeColumn(2); // To
						});

						// Header
						table.Header(header =>
						{
							header.Cell().Element(CellStyle).Text("Day").Bold();
							header.Cell().Element(CellStyle).Text("Date").Bold();
							header.Cell().Element(CellStyle).Text("Course Name").Bold();
							header.Cell().Element(CellStyle).Text("Location").Bold();
							header.Cell().Element(CellStyle).Text("From").Bold();
							header.Cell().Element(CellStyle).Text("To").Bold();

							static IContainer CellStyle(IContainer container)
							{
								return container.DefaultTextStyle(x => x.SemiBold()).Padding(5).Background(Colors.Grey.Lighten3).BorderBottom(1);
							}
						});

						// Data Rows
						foreach (var item in exams)
						{
							table.Cell().Element(CellStyle).Text(item.DayOfWeek);
							table.Cell().Element(CellStyle).Text(item.Date.ToString("yyyy-MM-dd"));
							table.Cell().Element(CellStyle).Text(item.CourseName);
							table.Cell().Element(CellStyle).Text(item.Location);
							table.Cell().Element(CellStyle).Text(item.StartTime);
							table.Cell().Element(CellStyle).Text(item.EndTime);

							static IContainer CellStyle(IContainer container)
							{
								return container.BorderBottom(0.5f).PaddingVertical(4).PaddingHorizontal(2);
							}
						}
					});
				});
			});

			return document.GeneratePdf();

			static string FormatTime(TimeSpan time) => DateTime.Today.Add(time).ToString("hh:mm tt");
		}


	}
}
