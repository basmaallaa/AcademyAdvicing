﻿using Academy.Core.Dtos;
using Academy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.ServicesInterfaces.ICoursesInterface
{
    public interface ICourseService
    {
   
        Task<CreateCourseDto> CreateCourseAsync(CreateCourseDto createCourseDto);

        Task<IEnumerable<CreateCourseDto>> SearchCoursesAsync(string? name/*, string? courseCode, int? creditHours, courseType? type, courseCategory? category*/);
       

        Task<CreateCourseDto> UpdateCourseAsync(int id, CreateCourseDto updateCourseDto);
        Task<CreateCourseDto> GetCourseByIdAsync(int id);
        Task<IEnumerable<GetCoursesDto>> GetAllCoursesAsync();
        Task<bool> DeleteCourseAsync(int id);

        Task<bool> IsCourseExistAsync(CreateCourseDto dto);



    }
}
