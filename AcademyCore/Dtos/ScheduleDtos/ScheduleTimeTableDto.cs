using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Dtos.ScheduleDtos
{
	public class ScheduleTimeTableDto
	{
		public int Id { get; set; }
		public string DayOfWeek { get; set; }
		public TimeOnly StartTime { get; set; }
		public TimeOnly EndTime { get; set; }
		public string Location { get; set; }


		//public string UploadedByName { get; set; }
		public string DoctorName { get; set; }
		//public int UploadedById { get; set; } //++++++++


		public string CourseName { get; set; }


	}
}
