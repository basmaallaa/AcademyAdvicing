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
using Academy.Repo.Data;

namespace Academy.Services.Services
{
    public class AvailableCourseService : IAvailableCourse
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly AcademyContext _academyDbContext;
        public AvailableCourseService(IUnitOfWork unitOfWork, IMapper mapper, AcademyContext academyContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _academyDbContext = academyContext;

        }

        

        public async Task CreateAvailableCourseAsync(AvailableCourse availableCourse)
        {
            await _unitOfWork.Repository<AvailableCourse>().AddAsync(availableCourse);
            await _unitOfWork.CompleteAsync();
        }


        public async Task<AvailableCourseDoctorDto> UpdateAvailableCourseAsync(int courseId, AvailableCourseDoctorDto dto)
        {
            // 1. جيب كل الصفوف المرتبطة بالكورس
            var existing = (await _unitOfWork
                .Repository<AvailableCourse>()
                .GetAllAsync(ac => ac.CourseId == courseId))
                .ToList();

            if (!existing.Any())
                return null;

            // 2. طبّق التحديثات العامة على كل صف
            existing.ForEach(ac => {
                ac.AcademicYears = dto.AcademicYears;
                ac.Semester = dto.Semester;
            });

            // 3. حدّد اللي اتحذف واللي هيتضاف
            var oldDoctorIds = existing.Select(ac => ac.DoctorId).ToHashSet();
            var newDoctorIds = dto.DoctorIds?.ToHashSet() ?? new HashSet<int>();

            var toRemove = oldDoctorIds.Except(newDoctorIds);
            var toAdd = newDoctorIds.Except(oldDoctorIds);

            // 4. احذف بس اللي اتشال من اللائحة
            foreach (var removeId in toRemove)
            {
                var ent = existing.First(ac => ac.DoctorId == removeId);
                _unitOfWork.Repository<AvailableCourse>().Delete(ent);
            }

            // 5. أضف بس اللي اتزاد جديد
            foreach (var addId in toAdd)
            {
                await _unitOfWork.Repository<AvailableCourse>()
                    .AddAsync(new AvailableCourse
                    {
                        CourseId = courseId,
                        AcademicYears = dto.AcademicYears,
                        Semester = dto.Semester,
                        DoctorId = addId
                    });
            }

            // 6. احفظ كل التغييرات
            await _unitOfWork.CompleteAsync();

            return dto;
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

       
        public async Task<IEnumerable<ViewAvailableCourseDto>> GetAllAvailableCoursesAsync()
        {
            var availableCourses = await _academyDbContext.Availablecourses
                .Include(ac => ac.Course)
                .Include(ac => ac.Doctor)
                .GroupBy(ac => new { ac.CourseId, ac.AcademicYears, ac.Semester })
                .Select(group => new ViewAvailableCourseDto
                {
                    Id = group.First().Id,
                    CourseId = group.Key.CourseId,
                    CourseName = group.First().Course.Name,
                    CourseCode = group.First().Course.CourseCode,
                    CreditHours = group.First().Course.CreditHours,
                    DoctorIds = group.Select(ac => ac.DoctorId).Distinct().ToList(),
                    DoctorName = group.Select(ac => ac.Doctor.Name).Distinct().ToList(),
                    AcademicYears = group.Key.AcademicYears,
                    Semester = (Semster)(group.Key.Semester == Semster.Fall ? 0 : 1),
                })
                .ToListAsync();

            // إرجاع قائمة فارغة إذا لم توجد كورسات
            return availableCourses ?? new List<ViewAvailableCourseDto>();
        }

        public async Task<bool> DeleteAvailableCourseAsync(int id)
        {
            var availableCourse = await _unitOfWork.Repository<AvailableCourse>().GetAsync(id);
            if (availableCourse == null) return false;

            _unitOfWork.Repository<AvailableCourse>().Delete(availableCourse);
            await _unitOfWork.CompleteAsync();
            return true;
        }


        public async Task<bool> IsCourseAvailableAsync(int courseId, int academicYear,  Semster semester)
        {
            var availableCourses = await _unitOfWork.Repository<AvailableCourse>().GetAllAsync();

            return availableCourses.Any(c => c.CourseId == courseId
                                          && c.AcademicYears == academicYear
                                          && c.Semester == semester);
        }



    }
}
