using Academy.Core;
using Academy.Core.Dtos;
using Academy.Core.Enums;
using Academy.Core.Models;
using Academy.Core.ServicesInterfaces.ICoursesInterface;
using Academy.Repo;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Services.Services.CourseService
{
   
public class CreateCourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        
        public CreateCourseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CreateCourseDto> CreateCourseAsync(CreateCourseDto createCourseDto)
        {
            if (createCourseDto == null)
                throw new ArgumentNullException(nameof(createCourseDto));

            var course = _mapper.Map<Course>(createCourseDto);

            await _unitOfWork.Repository<Course>().AddAsync(course);
            await _unitOfWork.CompleteAsync();

            // تحويل الـ Course المُنشأ إلى CreateCourseDto وإرجاعه
            var createdCourseDto = _mapper.Map<CreateCourseDto>(course);
            return createdCourseDto;
        }
        public async Task<CreateCourseDto> UpdateCourseAsync(int id, CreateCourseDto updateCourseDto)
        {
            var course = await _unitOfWork.Repository<Course>().GetAsync(id);
            if (course == null) return null;

            _mapper.Map(updateCourseDto, course);
            _unitOfWork.Repository<Course>().Update(course);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<CreateCourseDto>(course);
        }

        public async Task<CreateCourseDto> GetCourseByIdAsync(int id)
        {
            var course = await _unitOfWork.Repository<Course>().GetAsync(id);
            return course == null ? null : _mapper.Map<CreateCourseDto>(course);
        }

        public async Task<IEnumerable<CreateCourseDto>> GetAllCoursesAsync()
        {
            var courses = await _unitOfWork.Repository<Course>().GetAllAsync();
            return _mapper.Map<IEnumerable<CreateCourseDto>>(courses);
        }

        public async Task<IEnumerable<CreateCourseDto>> SearchCoursesAsync(
    string? name)
    /*string? courseCode,
    int? creditHours,
    courseType? type,
    courseCategory? category)*/
        {
            var coursesQuery =await _unitOfWork.Repository<Course>().GetAllAsync();

            if (!string.IsNullOrEmpty(name))
            {
                coursesQuery = coursesQuery.Where(c => c.Name.Contains(name));
            }

            /*if (!string.IsNullOrEmpty(courseCode))
            {
                coursesQuery = coursesQuery.Where(c => c.CourseCode.Contains(courseCode));
            }

            if (creditHours.HasValue)
            {
                coursesQuery = coursesQuery.Where(c => c.CreditHours == creditHours.Value);
            }

            if (type.HasValue)
            {
                coursesQuery = coursesQuery.Where(c => c.type == type.Value);
            }

            if (category.HasValue)
            {
                coursesQuery = coursesQuery.Where(c => c.category == category.Value);
            }*/

            var courses = await coursesQuery.ToListAsync();
            return _mapper.Map<IEnumerable<CreateCourseDto>>(courses);
        }





       /* public async Task<bool> DeleteCourseAsync(int id)
         {
             // اجلب الكيان الفعلي Course من قاعدة البيانات
             var course = await _unitOfWork.Repository<Course>().GetAsync(id);
             if (course == null) return false;

<<<<<<< HEAD

=======
             // مرر الـ id بدلاً من كائن course
             _unitOfWork.Repository<Course>().Delete(id);
             await _unitOfWork.CompleteAsync();

             return true;
         }*/

     



>>>>>>> 46b2f49194103baaca2e54b0d60967dcc35e727a
    }




}


