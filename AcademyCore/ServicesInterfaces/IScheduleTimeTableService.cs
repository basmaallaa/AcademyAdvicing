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
		Task<ScheduleTimeTableDto> AddAsync(CreateScheduleTimeTableDto createScheduleTimeTableDto);
		Task AddBulkScheduleAsync(CreateFullScheduleDto dto);
		Task DeleteAsync(int id);
		Task UpdateScheduleTimeTableAsync(int id, CreateScheduleTimeTableDto dto);
	}
}
