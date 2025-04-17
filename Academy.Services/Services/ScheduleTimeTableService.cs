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

namespace Academy.Services.Services
{
	public class ScheduleTimeTableService : IScheduleTimeTableService
	{
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;

		public ScheduleTimeTableService(IMapper mapper, IUnitOfWork unitOfWork)
		{
			_mapper = mapper;
			_unitOfWork = unitOfWork;
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



		public async Task<ScheduleTimeTableDto> AddAsync(CreateScheduleTimeTableDto dto)
		{
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
			if (availableCourse == null) return null;  // AvailableCourse not found

			var uploadedBy = await _unitOfWork.Repository<Coordinator>().GetAsync(dto.UploadedById);
			if (uploadedBy == null) return null;  // User (UploadedBy) not found

			// Map DTO to entity
			var scheduleTimeTable = _mapper.Map<ScheduleTimeTable>(dto);
			scheduleTimeTable.AvailableCourse = availableCourse;
			scheduleTimeTable.UploadedBy = uploadedBy;

			// Add entity to repository
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

		public async Task UpdateScheduleTimeTableAsync(int id, CreateScheduleTimeTableDto dto)
		{
			var existingSchedule = await _unitOfWork.Repository<ScheduleTimeTable>().GetAsync(id);

			if (existingSchedule == null)
				throw new ArgumentException("Schedule not found.");

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
			existingSchedule.AvailableCourseId = dto.AvailableCourseId;
			existingSchedule.DayOfWeek = dto.DayOfWeek;
			existingSchedule.StartTime = dto.StartTime;
			existingSchedule.EndTime = dto.EndTime;
			existingSchedule.Location = dto.Location;
			existingSchedule.UploadedById = dto.UploadedById;

			_unitOfWork.Repository<ScheduleTimeTable>().Update(existingSchedule);
			await _unitOfWork.CompleteAsync();

			//// Re-fetch to include navigation properties
			//var updatedSchedule = _unitOfWork.Repository<ScheduleTimeTable>()
			//	.GetAllAsync("AvailableCourse.Course,AvailableCourse.Doctor,UploadedBy")
			//	.FirstOrDefault(s => s.Id == id);

			//return _mapper.Map<ScheduleTimeTableDto>(updatedSchedule);
			//return _mapper.Map<ScheduleTimeTableDto>(existingSchedule);
		}

		public async Task AddBulkScheduleAsync(CreateFullScheduleDto dto)
		{
			foreach (var scheduleDto in dto.Courses)
			{
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
					UploadedById = dto.UploadedById,
					DayOfWeek = scheduleDto.DayOfWeek,
					StartTime = scheduleDto.StartTime,
					EndTime = scheduleDto.EndTime,
					Location = scheduleDto.Location
				};

				await _unitOfWork.Repository<ScheduleTimeTable>().AddAsync(schedule);
			}

			await _unitOfWork.CompleteAsync();
		}
	}
}
