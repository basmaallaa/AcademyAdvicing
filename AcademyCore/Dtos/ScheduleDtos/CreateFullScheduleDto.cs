using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Dtos.ScheduleDtos
{
	public class CreateFullScheduleDto
	{
		//public int UploadedById { get; set; }
		public List<CreateScheduleTimeTableDto> Courses { get; set; }
	}
}
