using Academy.Core.Models;
using Academy.Core;
using Academy.Core.ServicesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Academy.Core.Dtos;

namespace Academy.Services.Services
{
    public class DoctorCourseService : IDoctorCourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private IEnumerable<object> doctorCourseRepository;

        public DoctorCourseService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        public async Task AddDoctorCourseAsync(AvailableCourse doctorAvailableCourses)
        {
            await _unitOfWork.Repository<AvailableCourse>().AddAsync(doctorAvailableCourses);
            await _unitOfWork.CompleteAsync();
        }

        //public async Task AddDoctorCoursesAsync(DoctorCourseDto dto)
        //{
        //    var existing = await _unitOfWork.Repository<DoctorCourses>()
        //.FirstOrDefaultAsync(x => x.DoctorId == dto.DoctorId && x.CourseId == dto.CourseId);

        //    if (existing != null)
        //    {
        //        // ممكن ترجع أو تسكت أو تحدثه لو محتاج
        //        return;
        //    }

        //    var entity = _mapper.Map<DoctorCourses>(dto);
        //    await _unitOfWork.Repository<DoctorCourses>().AddAsync(entity);
        //    await _unitOfWork.CompleteAsync();
        //}


    }
}
