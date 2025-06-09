using Academy.Core.Dtos.FinalTimeTableDtos;
using Academy.Core.Dtos.ScheduleDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.ServicesInterfaces
{
	public interface IFinalExamTimeTableService
	{
		Task<IEnumerable<FinalExamTimeTableDto>> GetAllAsync();
		Task<FinalExamTimeTableDto> GetByIdAsync(int id);
		Task<FinalExamTimeTableDto> AddAsync(CreateFinalExamTimeTableDto dto , int coordinatorId);
		Task AddBulkFinalTimeTableAsync (CreateFullFinalTimeTableDto dto , int coordinatorId);
		Task DeleteAsync(int id);
		Task UpdateFinalTimeTableAsync(int id, EditFinalExamTimeTableDto dto, int coordinatorId);
		Task<List<FinalExamTimeTableDto>> GetStudentFinalExamSchedule(int studentId);

		byte[] GenerateFinalExamPdf(List<FinalExamTimeTableDto> exams, string studentName, string level);

	}
}
