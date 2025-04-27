using Academy.Core;
using Academy.Core.Dtos;
using Academy.Core.Models;
using Academy.Core.Models.Identity;
using Academy.Core.Service;
using Academy.Core.ServicesInterfaces;
using Academy.Repo.Data;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Academy.Services.Services
{
	public class StudentService : IStudentService
	{
		private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly AcademyContext _academyDbContext;
        private readonly IFileService _fileService;

        public StudentService(IMapper mapper , IUnitOfWork unitOfWork, UserManager<AppUser> userManager, ITokenService tokenService, AcademyContext academyDbContext, IFileService fileService)
        {
			_mapper = mapper;
			_unitOfWork = unitOfWork;
            _userManager = userManager;
            _tokenService = tokenService;
            _academyDbContext = academyDbContext;
           _fileService = fileService;
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

        //public async Task<StudentDto> AddStudentAsync(StudentDto studentDto)
        //{
        //	// map dto to model
        //	var student = _mapper.Map<Student>(studentDto);

        //	// add the student to the repo 
        //	await _unitOfWork.Repository<Student>().AddAsync(student);

        //	await _unitOfWork.CompleteAsync();

        //	// map back the model to dto
        //	return _mapper.Map<StudentDto>(student);
        //}

        public async Task<StudentDtoID> AddStudentAsync( StudentDto model)
        {
            
            var existingStudent = await _academyDbContext.Students
                .FirstOrDefaultAsync(s => s.Email == model.Email);
            if (existingStudent != null)
                throw new Exception("Student with this email is already registered.");

            string? imageFileName = null;

            if (model.ImageFile is not null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

                if (model.ImageFile.Length > 1 * 1024 * 1024)
                    throw new ArgumentException("Image size should not exceed 1 MB.");

                imageFileName = await _fileService.SaveFileAsync(model.ImageFile, allowedExtensions);
            }



            var student = new Student
            {
                Name = model.Name,
                UserName = model.UserName,
                Email = model.Email,
                Level = model.Level,
                Status = model.Status,
                GPA = model.GPA,
                CompeletedHours = model.CompeletedHours,
                ImagePath = imageFileName,
                PhoneNumber = model.PhoneNumber

            };

            // إضافة الطالب إلى قاعدة البيانات الخاصة بالأكاديمية
            _academyDbContext.Students.Add(student);
            await _academyDbContext.SaveChangesAsync();

            // الآن سننشئ المستخدم في Identity
            var appUser = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                DisplayName = model.Name,
                PhoneNumber= model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(appUser, "Student@123"); // تأكدي من تعيين كلمة مرور مؤقتة أو إرسال بريد إلكتروني لتغييرها
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(x => x.Description)));

            // إضافة دور الطالب إلى Identity
            var roleResult = await _userManager.AddToRoleAsync(appUser, "Student");
            if (!roleResult.Succeeded)
                throw new Exception(string.Join(", ", roleResult.Errors.Select(x => x.Description)));

            // إنشاء توكين للمستخدم الجديد
            var token = await _tokenService.CreateTokenAsync(appUser, _userManager);

            // إنشاء كائن StudentDto لإرجاع تفاصيل الطالب والتوكين
            var studentDtoID = new StudentDtoID
            {
                Id = student.Id,
                Name = student.Name,
                UserName = student.UserName,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber,
                ImagePath= imageFileName,
                Level = student.Level,
                Status = student.Status,
                GPA = student.GPA,
                CompeletedHours = student.CompeletedHours,
                Token = token
            };

            return studentDtoID;
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

       

        public async Task<IEnumerable<StudentDtoID>> SearchStudentsAsync(string? searchTerm)
        {
            // Fetch data first
            var studentsList = await _unitOfWork.Repository<Student>().GetAllAsync();
            var studentsQuery = studentsList.AsQueryable(); // Convert to IQueryable for filtering

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower(); // Normalize input

                // Parse searchTerm before filtering
                bool isInt = int.TryParse(searchTerm, out int id);
                bool isFloat = float.TryParse(searchTerm, out float gpa);
                bool isHours = int.TryParse(searchTerm, out int hours);

                studentsQuery = studentsQuery.Where(c =>
                    c.Name.ToLower().Contains(searchTerm) ||
                    c.UserName.ToLower().Contains(searchTerm) ||
                    c.PhoneNumber.ToLower().Contains(searchTerm) ||
                    c.Level.ToLower().Contains(searchTerm) ||
                    c.Status.ToString().ToLower().Contains(searchTerm) ||
                    (isInt && c.Id == id) ||            // If number, check ID
                    (isFloat && c.GPA == gpa) ||        // If float, check GPA
                    (isHours && c.CompeletedHours == hours) // If int, check Completed Hours
                );
            }

            var students = studentsQuery.ToList(); // Execute filtering in memory
            return _mapper.Map<IEnumerable<StudentDtoID>>(students);
        }

        /* Assign courses to the student */





    }
}
