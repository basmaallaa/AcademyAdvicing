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
    public class DoctorProfile : Profile
    {
        public DoctorProfile() 
        {
            CreateMap<Doctor, DoctorDto>().ReverseMap(); //el reverce deh 3lshan lw h map el 3aks
            CreateMap<Doctor, DoctorDtoID>().ReverseMap();
        }
    }
}
