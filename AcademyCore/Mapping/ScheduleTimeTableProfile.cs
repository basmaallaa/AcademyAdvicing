using Academy.Core.Dtos.ScheduleDtos;
using Academy.Core.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Mapping
{
	public class ScheduleTimeTableProfile : Profile
	{
		public ScheduleTimeTableProfile()
		{
			CreateMap<CreateScheduleTimeTableDto, ScheduleTimeTable>()
			.ForMember(stt => stt.AvailableCourseId, opt => opt.MapFrom(src => src.AvailableCourseId))
			.ForMember(stt => stt.DayOfWeek, opt => opt.MapFrom(src => src.DayOfWeek))
			.ForMember(stt => stt.StartTime, opt => opt.MapFrom(src => src.StartTime))
			.ForMember(stt => stt.EndTime, opt => opt.MapFrom(src => src.EndTime))
			.ForMember(stt => stt.Location, opt => opt.MapFrom(src => src.Location))
			.ForMember(stt => stt.UploadedById, opt => opt.MapFrom(src => src.UploadedById))
			;

			CreateMap<ScheduleTimeTable, ScheduleTimeTableDto>()
			.ForMember(sttd => sttd.CourseName, opt => opt.MapFrom(src => src.AvailableCourse.Course.Name))
			.ForMember(sttd => sttd.DoctorName, opt => opt.MapFrom(src => src.AvailableCourse.Doctor.Name))
			//.ForMember(sttd => sttd.UploadedByName, opt => opt.MapFrom(src=>src.UploadedBy.Name))
			.ForMember(sttd => sttd.UploadedById, opt => opt.MapFrom(src => src.UploadedById))
			.ForMember(sttd => sttd.DayOfWeek, opt => opt.MapFrom(src => src.DayOfWeek))
			.ForMember(sttd => sttd.StartTime, opt => opt.MapFrom(src => src.StartTime))
			.ForMember(sttd => sttd.EndTime, opt => opt.MapFrom(src => src.EndTime))
			.ForMember(sttd => sttd.Location, opt => opt.MapFrom(src => src.Location));
		}
		}
	}
