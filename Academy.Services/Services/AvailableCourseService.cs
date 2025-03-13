using Academy.Core.Dtos;
using Academy.Core.Models;
using Academy.Core;
using Academy.Core.ServicesInterfaces;
using Academy.Core.ServicesInterfaces.ICoursesInterface;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Academy.Core.Enums;

namespace Academy.Services.Services
{
    public class AvailableCourseService : IAvailableCourse
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AvailableCourseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /* public async Task<AvailableCourseDto> CreateAvailableCourseAsync(AvailableCourseDto availableCourseDto)
         {
             var availableCourse = _mapper.Map<AvailableCourse>(availableCourseDto);
             await _unitOfWork.Repository<AvailableCourse>().AddAsync(availableCourse);
             await _unitOfWork.CompleteAsync();
             return _mapper.Map<AvailableCourseDto>(availableCourse);
         }*/
        public async Task<AvailableCourseDto> CreateAvailableCourseAsync(AvailableCourseDto availableCourseDto)
        {
            // الأول نشوف الكورس ده موجود ولا لأ
            var existingCourse = await _unitOfWork.Repository<Course>().GetAsync(availableCourseDto.CourseId);
            if (existingCourse == null)
            {
                throw new Exception($"CourseId {availableCourseDto.CourseId} مش موجود!");
            }

            // تحويل الـ DTO إلى كيان AvailableCourse
            var availableCourse = _mapper.Map<AvailableCourse>(availableCourseDto);

            // إضافة الكورس المتاح
            await _unitOfWork.Repository<AvailableCourse>().AddAsync(availableCourse);

            // حفظ التغييرات
            await _unitOfWork.CompleteAsync();

            // تحويل الكيان مرة تانية إلى DTO وإرجاعه
            return _mapper.Map<AvailableCourseDto>(availableCourse);
        }




        public async Task<AvailableCourseDto> UpdateAvailableCourseAsync(int id, AvailableCourseDto updateAvailableCourseDto)
        {
            var availableCourse = await _unitOfWork.Repository<AvailableCourse>().GetAsync(id);
            if (availableCourse == null) return null;

            _mapper.Map(updateAvailableCourseDto, availableCourse);
            _unitOfWork.Repository<AvailableCourse>().Update(availableCourse);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<AvailableCourseDto>(availableCourse);
        }

        public async Task<ViewAvailableCourseDto> GetAvailableCourseByIdAsync(int id)
        {
            var availableCourse = await _unitOfWork.Repository<AvailableCourse>()
                .GetAsync(id);  // استرجاع الكورس المتاح فقط بدون Include

            if (availableCourse == null)
                return null;

            // جلب بيانات الكورس بشكل منفصل بدون Include
            var course = await _unitOfWork.Repository<Course>().GetAsync(availableCourse.CourseId);

            // تحويل البيانات للـ DTO
            var availableCourseDto = _mapper.Map<ViewAvailableCourseDto>(availableCourse);

            // إضافة بيانات الكورس بشكل يدوي
            if (course != null)
            {
                availableCourseDto.CourseName = course.Name;
                availableCourseDto.CourseCode = course.CourseCode;
                availableCourseDto.CreditHours = course.CreditHours;
            }

            return availableCourseDto;
        }

        //public async Task<AvailableCourseViewDto> GetAvailableCourseByIdAsync(int id)
        //{
        //    var availableCourse = await _unitOfWork.Repository<AvailableCourse>()
        //        .GetQueryable()
        //        .Include(ac => ac.Course)  // تحميل بيانات الكورس مباشرة
        //        .FirstOrDefaultAsync(ac => ac.Id == id);

        //    return availableCourse == null ? null : _mapper.Map<AvailableCourseViewDto>(availableCourse);
        //}


        public async Task<IEnumerable<ViewAvailableCourseDto>> GetAllAvailableCoursesAsync()
        {
            var availableCourses = await _unitOfWork.Repository<AvailableCourse>().GetAllAsync();

            var result = availableCourses.Select(ac => new ViewAvailableCourseDto
            {
                Id = ac.Id,
                AcademicYears = ac.AcademicYears,
                Semester = ac.Semester,
                CourseId = ac.CourseId,
                CourseName = ac.Course != null ? ac.Course.Name : "Unknown",
                CourseCode = ac.Course != null ? ac.Course.CourseCode : "N/A",
                CreditHours = ac.Course != null ? ac.Course.CreditHours : 0
            }).ToList();

            return result;
        }





        public async Task<bool> DeleteAvailableCourseAsync(int id)
        {
            var availableCourse = await _unitOfWork.Repository<AvailableCourse>().GetAsync(id);
            if (availableCourse == null) return false;

            _unitOfWork.Repository<AvailableCourse>().Delete(availableCourse);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    
}
}
