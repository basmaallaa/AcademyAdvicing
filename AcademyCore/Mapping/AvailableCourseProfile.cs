using Academy.Core.Dtos;
using Academy.Core.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Mapping
{
    public  class AvailableCourseProfile : Profile
    {
        public AvailableCourseProfile() {

            CreateMap<AvailableCourse, AvailableCourseDto>().ReverseMap();
            CreateMap<AvailableCourse,ViewAvailableCourseDto>().ReverseMap();
            CreateMap<AvailableCourse, ViewAvailableCourseDto>().ReverseMap();
            CreateMap<AvailableCourseDoctorDto, AvailableCourse>()
           .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.DoctorIds));
            CreateMap<AvailableCourse, AvailableCourseDoctorDto>();

        }
    }
}
