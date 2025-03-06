using Academy.Core;
using Academy.Core.Dtos;
using Academy.Core.Enums;
using Academy.Core.Models;
using Academy.Core.ServicesInterfaces.ICoursesInterface;
using Academy.Repo;
using AutoMapper;
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



 

     
    }



}


