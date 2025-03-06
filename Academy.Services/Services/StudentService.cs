using Academy.Core;
using Academy.Core.Dtos;
using Academy.Core.Models;
using Academy.Core.ServicesInterfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Services.Services
{
	public class StudentService : IStudentService
	{
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;

		public StudentService(IMapper mapper , IUnitOfWork unitOfWork)
        {
			_mapper = mapper;
			_unitOfWork = unitOfWork;
		}

		public void DeleteStudentAsync(int id)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync()
		{
			return _mapper.Map<IEnumerable<StudentDto>>(await _unitOfWork.Repository<Student>().GetAllAsync());
		}

		public async Task<StudentDto> GetStudentByIdAsync(int id)
		{
			var student = await _unitOfWork.Repository<Student>().GetAsync(id);
			var studentMapped = _mapper.Map<StudentDto>(student);
			return studentMapped;
		}

		public Task UpdateStudentAsync(StudentDto studentDto)
		{
			throw new NotImplementedException();
		}

		#region #سيبيلي العبط ده هبقى ابص عليه تاني يا رحومه
		//public async Task UpdateStudentAsync(StudentDto studentDto)
		//{
		//	var studentMapped =_mapper.Map<Student>(studentDto);
		//	var student = _unitOfWork.Repository<Student>().Update(studentMapped);
		//} 
		#endregion
	}
}
