using Academy.Core.Dtos.ScheduleDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.ServicesInterfaces
{
	public interface IScheduleTimeTableService
	{
		Task<IEnumerable<ScheduleTimeTableDto>> GetAllAsync();
		Task<ScheduleTimeTableDto> GetByIdAsync(int id);
		Task<ScheduleTimeTableDto> AddAsync(CreateScheduleTimeTableDto createScheduleTimeTableDto , int coordinatorId);
		Task AddBulkScheduleAsync(CreateFullScheduleDto dto, int coordinatorId);
		Task DeleteAsync(int id);
		Task UpdateScheduleTimeTableAsync(int id, EditScheduleTimeTableDto dto , int coordinatorId);

		Task<List<ScheduleTimeTableDto>> GetStudentSchedule(int studentId);
		byte[] GenerateStudentSchedulePdf(List<ScheduleTimeTableDto> schedule, string studentName, string level);
	}
}
