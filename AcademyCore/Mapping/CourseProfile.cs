using AutoMapper;
using Academy.Core.Dtos;
using Academy.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Mapping
{
    public class CourseProfile :Profile
    {
        public CourseProfile() {

            CreateMap<Course, CreateCourseDto>().ReverseMap();
        
        }
    }
}
