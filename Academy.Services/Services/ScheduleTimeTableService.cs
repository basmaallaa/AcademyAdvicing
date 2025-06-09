using Academy.Core.Dtos.ScheduleDtos;
using Academy.Core.Models;
using Academy.Core;
using Academy.Core.ServicesInterfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Academy.Repo.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Academy.Core.Dtos.ScheduleDtos;


namespace Academy.Services.Services
{
	public class ScheduleTimeTableService : IScheduleTimeTableService
	{
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
		private readonly AcademyContext _academyContext;

		public ScheduleTimeTableService(IMapper mapper, IUnitOfWork unitOfWork , AcademyContext academyContext)
		{
			_mapper = mapper;
			_unitOfWork = unitOfWork;
			_academyContext = academyContext;
		}
		public async Task<IEnumerable<ScheduleTimeTableDto>> GetAllAsync()
		{



			var TimeTablesq = await _unitOfWork.Repository<ScheduleTimeTable>()
				.GetAllAsync();

			var timeTables = await TimeTablesq
			   .Include(st => st.UploadedBy)
			   .Include(st => st.AvailableCourse)
					.ThenInclude(ac => ac.Course)
				.Include(st => st.AvailableCourse)
					.ThenInclude(ac => ac.Doctor)
			   .ToListAsync();

			return _mapper.Map<IEnumerable<ScheduleTimeTableDto>>(timeTables);

		}

		public async Task<ScheduleTimeTableDto> GetByIdAsync(int id)
		{
			var TimeTabless = await _unitOfWork.Repository<ScheduleTimeTable>().GetAllAsync();

			var timeTable = await TimeTabless
			.Include(st => st.UploadedBy)
			.Include(st => st.AvailableCourse)
				.ThenInclude(ac => ac.Course)
			.Include(st => st.AvailableCourse)
				.ThenInclude(ac => ac.Doctor)
			.FirstOrDefaultAsync(st => st.Id == id);

			return _mapper.Map<ScheduleTimeTableDto>(timeTable);



			//var scheduleTimeTable = await _unitOfWork.Repository<ScheduleTimeTable>().GetAsync(id);
			//if (scheduleTimeTable == null) return null;  // Not found

			//return _mapper.Map<ScheduleTimeTableDto>(scheduleTimeTable);
		}



		public async Task DeleteAsync(int id)
		{
			var timeTable = await _unitOfWork.Repository<ScheduleTimeTable>().GetAsync(id);
			if (timeTable == null) throw new KeyNotFoundException($"Time Table with ID {id} not found.");

			_unitOfWork.Repository<ScheduleTimeTable>().Delete(timeTable);
			await _unitOfWork.CompleteAsync();

		}



		public async Task<ScheduleTimeTableDto> AddAsync(CreateScheduleTimeTableDto dto, int coordinatorId)
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

			// Check if there's already a course at the same time and location
			var allSchedules = await _unitOfWork.Repository<ScheduleTimeTable>().GetAllAsync();
			var conflict = allSchedules.Any(s =>
				s.DayOfWeek == dto.DayOfWeek &&
				s.StartTime == dto.StartTime &&
				s.EndTime == dto.EndTime &&
				s.Location.ToLower() == dto.Location.ToLower());

			if (conflict)
			{
				throw new ArgumentException("There is already a course scheduled at this time and location.");
			}

			var availableCourse = await _unitOfWork.Repository<AvailableCourse>().GetAsync(dto.AvailableCourseId);
			if (availableCourse == null) return null;

			var uploadedBy = await _unitOfWork.Repository<Coordinator>().GetAsync(coordinatorId);
			if (uploadedBy == null) return null;

			var scheduleTimeTable = _mapper.Map<ScheduleTimeTable>(dto);
			scheduleTimeTable.AvailableCourse = availableCourse;
			scheduleTimeTable.UploadedBy = uploadedBy;

			await _unitOfWork.Repository<ScheduleTimeTable>().AddAsync(scheduleTimeTable);
			await _unitOfWork.CompleteAsync();

			return _mapper.Map<ScheduleTimeTableDto>(scheduleTimeTable);
		}


		//public async Task<ScheduleTimeTableDto> UpdateScheduleTimeTableAsync(int id, CreateScheduleTimeTableDto dto)
		//{
		//	var existingSchedule = await _unitOfWork.Repository<ScheduleTimeTable>().GetAsync(id);

		//	if (existingSchedule == null)
		//		throw new ArgumentException("Schedule not found.");

		//	// Check for conflict with other schedules (excluding the current one)
		//	var allSchedules = await _unitOfWork.Repository<ScheduleTimeTable>().GetAllAsync();
		//	var conflict = allSchedules.Any(s =>
		//		s.Id != id &&
		//		s.DayOfWeek == dto.DayOfWeek &&
		//		s.StartTime == dto.StartTime &&
		//		s.EndTime == dto.EndTime &&
		//		s.Location.ToLower() == dto.Location.ToLower()
		//	);

		//	if (conflict)
		//		throw new ArgumentException("There is already a course scheduled at this time and location.");

		//	existingSchedule.AvailableCourseId = dto.AvailableCourseId;
		//	existingSchedule.DayOfWeek = dto.DayOfWeek;
		//	existingSchedule.StartTime = dto.StartTime;
		//	existingSchedule.EndTime = dto.EndTime;
		//	existingSchedule.Location = dto.Location;
		//	existingSchedule.UploadedById = dto.UploadedById;

		//	 _unitOfWork.Repository<ScheduleTimeTable>().Update(existingSchedule);
		//	await _unitOfWork.CompleteAsync();

		//	return _mapper.Map<ScheduleTimeTableDto>(existingSchedule);
		//}

		public async Task UpdateScheduleTimeTableAsync(int id, EditScheduleTimeTableDto dto, int coordinatorId)
		{
			var existingSchedule = await _unitOfWork.Repository<ScheduleTimeTable>().GetAsync(id);

			if (existingSchedule == null)
				throw new ArgumentException("Schedule not found.");


			if (dto.StartTime == dto.EndTime)
			{
				throw new ArgumentException("Start time and end time cannot be the same.");
			}

			if (dto.StartTime > dto.EndTime)
			{
				throw new ArgumentException("Start time must be before end time.");
			}


			// Check for conflict with other schedules (excluding the current one)
			var allSchedules = await _unitOfWork.Repository<ScheduleTimeTable>().GetAllAsync();
			var conflict = allSchedules.Any(s =>
				s.Id != id &&
				s.DayOfWeek == dto.DayOfWeek &&
				s.StartTime == dto.StartTime &&
				s.EndTime == dto.EndTime &&
				s.Location.ToLower() == dto.Location.ToLower()
			);

			if (conflict)
				throw new ArgumentException("There is already a course scheduled at this time and location.");

			// Update values
			//existingSchedule.AvailableCourseId = dto.AvailableCourseId;
			existingSchedule.DayOfWeek = dto.DayOfWeek;
			existingSchedule.StartTime = dto.StartTime;
			existingSchedule.EndTime = dto.EndTime;
			existingSchedule.Location = dto.Location;
			existingSchedule.UploadedById =coordinatorId;

			_unitOfWork.Repository<ScheduleTimeTable>().Update(existingSchedule);
			await _unitOfWork.CompleteAsync();

			
		}

		public async Task AddBulkScheduleAsync(CreateFullScheduleDto dto , int coordinatorId)
		{
			foreach (var scheduleDto in dto.Courses)
			{
				// check Time Logic
				if (scheduleDto.StartTime == scheduleDto.EndTime)
				{
					throw new Exception($"Start time and end time cannot be the same for course at {scheduleDto.DayOfWeek} in {scheduleDto.Location}.");
				}

				if (scheduleDto.StartTime > scheduleDto.EndTime)
				{
					throw new Exception($"Start time must be before end time for course at {scheduleDto.DayOfWeek} in {scheduleDto.Location}.");
				}

				// Check for time aw location conflicts
				var conflict = await _unitOfWork.Repository<ScheduleTimeTable>().GetAllAsync();
				var isConflict = conflict.Any(s =>
					s.DayOfWeek == scheduleDto.DayOfWeek &&
					s.StartTime == scheduleDto.StartTime &&
					s.EndTime == scheduleDto.EndTime &&
					s.Location.ToLower() == scheduleDto.Location.ToLower()
				);

				if (isConflict)
				{
					throw new Exception($"Conflict at {scheduleDto.DayOfWeek} {scheduleDto.StartTime}-{scheduleDto.EndTime} in {scheduleDto.Location}");
				}

				var schedule = new ScheduleTimeTable
				{
					AvailableCourseId = scheduleDto.AvailableCourseId,
					UploadedById = coordinatorId,
					DayOfWeek = scheduleDto.DayOfWeek,
					StartTime = scheduleDto.StartTime,
					EndTime = scheduleDto.EndTime,
					Location = scheduleDto.Location
				};

				await _unitOfWork.Repository<ScheduleTimeTable>().AddAsync(schedule);
			}

			await _unitOfWork.CompleteAsync();
		}


		public async Task<List<ScheduleTimeTableDto>> GetStudentSchedule(int studentId)
		{
			// Step 1: Get student's assigned courses
			var student = await _unitOfWork.Repository<Student>().FirstOrDefaultAsync(s => s.Id == studentId);
			if (student == null)
				return null;

			var assignedCourses = await _unitOfWork.Repository<AssignedCourse>().GetAllAsync(ac => ac.StudentId == studentId);

			var courseIds = assignedCourses.Select(ac => ac.CourseId).Distinct().ToList();

			if (!courseIds.Any())
				return new List<ScheduleTimeTableDto>();

			// Step 2: Get available courses related to the student's assigned courseIds
			var availableCourses = await _unitOfWork.Repository<AvailableCourse>().GetAllAsync(ac => courseIds.Contains(ac.CourseId));

			var availableCourseIds = availableCourses.Select(ac => ac.Id).Distinct().ToList();

			if (!availableCourseIds.Any())
				return new List<ScheduleTimeTableDto>();

			// Step 3: Get schedule entries for those available courses
			var timeTables = await _academyContext.ScheduleTimeTable
			.Where(st => availableCourseIds.Contains(st.AvailableCourseId ?? 0))
			.Include(st => st.UploadedBy)
			.Include(st => st.AvailableCourse)
				.ThenInclude(ac => ac.Course)
			.Include(st => st.AvailableCourse)
				.ThenInclude(ac => ac.Doctor)
			.ToListAsync();

			return _mapper.Map<List<ScheduleTimeTableDto>>(timeTables);
		}


		public byte[] GenerateStudentSchedulePdf(List<ScheduleTimeTableDto> schedule, string studentName, string level)
		{
			QuestPDF.Settings.License = LicenseType.Community;

			var document = Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Margin(30);
					page.Size(PageSizes.A4);
					page.PageColor(Colors.White);
					page.DefaultTextStyle(x => x.FontSize(12));

					page.Header().Column(col =>
					{
						col.Item().Text($"Student Name: {studentName}").Bold().FontSize(14);
						col.Item().Text($"Level: {level}").FontSize(12);
						col.Item().PaddingVertical(10).Text("Student Weekly Schedule")
							.FontSize(20).Bold().FontColor(Colors.Blue.Medium);
					});

					page.Content().Table(table =>
					{
						// Define column widths
						table.ColumnsDefinition(columns =>
						{
							columns.RelativeColumn(2); // Day
							columns.RelativeColumn(3); // Course
							columns.RelativeColumn(5); // Time (wider)
							columns.RelativeColumn(3); // Doctor
							columns.RelativeColumn(3); // Location
						});

						// Header row
						table.Header(header =>
						{
							header.Cell().Element(CellStyle).Text("Day").Bold();
							header.Cell().Element(CellStyle).Text("Course").Bold();
							header.Cell().Element(CellStyle).Text("Time").Bold();
							header.Cell().Element(CellStyle).Text("Doctor").Bold();
							header.Cell().Element(CellStyle).Text("Location").Bold();

							static IContainer CellStyle(IContainer container)
							{
								return container.DefaultTextStyle(x => x.SemiBold()).Padding(5).Background(Colors.Grey.Lighten3).BorderBottom(1);
							}
						});

						// Data rows
						foreach (var item in schedule)
						{
							table.Cell().Element(CellStyle).Text(item.DayOfWeek);
							table.Cell().Element(CellStyle).Text(item.CourseName);
							table.Cell().Element(CellStyle).Text($"{item.StartTime} - {item.EndTime}");
							table.Cell().Element(CellStyle).Text(item.DoctorName);
							table.Cell().Element(CellStyle).Text(item.Location);

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
