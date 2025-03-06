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

        //public Task<IEnumerable<StudentDtoID>> SearchStudentByNameAsync(string name)
        //{
        //    if (string.IsNullOrWhiteSpace(name))
        //        return Task.FromResult<IEnumerable<StudentDtoID>>(new List<StudentDtoID>()); // Return empty list if name is null/empty

        //    //var studentsQuery = _unitOfWork.Repository<Student>().GetQueryable();
        //    IQueryable<Student> studentsQuery = (IQueryable<Student>)_unitOfWork.Repository<Student>().GetQueryable();


        //    string loweredName = name.ToLower(); // Convert search input to lowercase

        //    studentsQuery = studentsQuery.Where(s => s.Name.ToLower().Contains(loweredName)); // Case-insensitive search

        //    var result = studentsQuery.ToList(); // Execute query
        //    return Task.FromResult(_mapper.Map<IEnumerable<StudentDtoID>>(result));
        //}



        //public Task<IEnumerable<StudentDtoID>> SearchStudentsAsync(string? query, double? gpa, string? level, int? completedHours)
        //{
        //    IQueryable<Student> studentsQuery = (IQueryable<Student>)_unitOfWork.Repository<Student>().GetQueryable();

        //    if (!string.IsNullOrEmpty(query))
        //    {
        //        studentsQuery = studentsQuery.Where(s =>
        //            s.Id.ToString().Contains(query) ||
        //            s.Name.Contains(query) ||
        //            s.PhoneNumber.Contains(query) ||
        //            s.UserName.Contains(query));
        //    }

        //    if (gpa.HasValue)
        //        studentsQuery = studentsQuery.Where(s => s.GPA == gpa.Value);

        //    if (!string.IsNullOrEmpty(level))
        //        studentsQuery = studentsQuery.Where(s => s.Level.Contains(level));

        //    if (completedHours.HasValue)
        //        studentsQuery = studentsQuery.Where(s => s.CompeletedHours == completedHours.Value);

        //    var result = studentsQuery.ToList();  // Execute query
        //    return Task.FromResult(_mapper.Map<IEnumerable<StudentDtoID>>(result));
        //}


        //public Task<IEnumerable<StudentDtoID>> SearchStudentByNameAsync(string name)
        //{
        //    if (string.IsNullOrWhiteSpace(name))
        //        return Task.FromResult<IEnumerable<StudentDtoID>>(new List<StudentDtoID>()); // Return empty list if name is null/empty

        //    var studentsQuery = _unitOfWork.Repository<Student>().GetQueryable();

        //    string loweredName = name.ToLower(); // Convert search input to lowercase

        //    studentsQuery = studentsQuery.Where(s => s.Name.ToLower().Contains(loweredName)); // Case-insensitive search

        //    var result = studentsQuery.ToList(); // Execute query
        //    return Task.FromResult(_mapper.Map<IEnumerable<StudentDtoID>>(result));
        //}


    }
}
