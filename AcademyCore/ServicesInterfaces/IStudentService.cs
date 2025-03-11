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
		Task<IEnumerable<StudentDtoID>> GetAllStudentsAsync();
		Task<StudentDtoID> GetStudentByIdAsync(int id);
		Task UpdateStudentAsync(StudentDtoID studentDtoid);
		Task<StudentDto> AddStudentAsync(StudentDto studentDto);

        Task DeleteStudentAsync(int id);
		Task<IEnumerable<StudentDtoID>> SearchStudentsAsync(string? searchTerm);

    }
}
