﻿using Academy.Core.Dtos;
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
		}
	}
}
