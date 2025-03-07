using Academy.Core;
using Academy.Core.Dtos;
using Academy.Core.Models;
using Academy.Core.ServicesInterfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Services.Services
{
	public class StudentService : IStudentService
	{
		private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StudentService(IMapper mapper , IUnitOfWork unitOfWork)
        {
			_mapper = mapper;
			_unitOfWork = unitOfWork;
		}

       


        public async Task<IEnumerable<StudentDtoID>> GetAllStudentsAsync()
		{
			return _mapper.Map<IEnumerable<StudentDtoID>>(await _unitOfWork.Repository<Student>().GetAllAsync());
		}

		public async Task<StudentDtoID> GetStudentByIdAsync(int id)
		{
			var student = await _unitOfWork.Repository<Student>().GetAsync(id);
			var studentMapped = _mapper.Map<StudentDtoID>(student);
			return studentMapped;
		}

		public async Task<StudentDto> AddStudentAsync(StudentDto studentDto)
		{
			// map dto to model
			var student = _mapper.Map<Student>(studentDto);

			// add the student to the repo 
			await _unitOfWork.Repository<Student>().AddAsync(student);

			await _unitOfWork.CompleteAsync();

			// map back the model to dto
			return _mapper.Map<StudentDto>(student);
		}



		
        public async Task UpdateStudentAsync(StudentDtoID studentDtoid)
        {
            var existingStudent = await _unitOfWork.Repository<Student>().GetAsync(studentDtoid.Id);

            if (existingStudent == null)
            {
                throw new KeyNotFoundException($"Student with ID {studentDtoid.Id} not found");
            }
            // Map updated properties from DTO to the existing entity
            _mapper.Map(studentDtoid, existingStudent);

            // Update student in repository
            _unitOfWork.Repository<Student>().Update(existingStudent);

            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteStudentAsync(int id)
        {
            // retrive the student from the DB
            var student = await _unitOfWork.Repository<Student>().GetAsync(id);

            if (student == null)
                throw new KeyNotFoundException($"Student with ID {id} not found.");

            _unitOfWork.Repository<Student>().Delete(student);
            await _unitOfWork.CompleteAsync();
        }

        

    }
}
