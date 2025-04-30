using Academy.Core.Dtos.MaterialsDtos;
using Academy.Core.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Mapping
{
    public class MaterialProfile : Profile
	{
		public MaterialProfile()
		{
			CreateMap<Material, MaterialDto>().ReverseMap();
			CreateMap<Material, MaterialViewDto>()
				.ForMember(dest => dest.UploadedByName, opt => opt.MapFrom(src => src.UploadedBy.Name))
				.ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name));

			CreateMap<CreateMaterialDto, Material>();



		}
	}
}
