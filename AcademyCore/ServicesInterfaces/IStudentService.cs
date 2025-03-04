using Academy.Core.Dtos;
using Academy.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.ServicesInterfaces
{
	public interface IStudentService
	{
		Task<IEnumerable<StudentDto>> GetAllStudentsAsync();
		Task<StudentDto> GetStudentByIdAsync(int id);
		Task UpdateStudentAsync(StudentDto studentDto);
		void DeleteStudentAsync(int id);

	}
}
