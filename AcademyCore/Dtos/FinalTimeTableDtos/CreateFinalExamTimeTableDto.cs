using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Dtos.FinalTimeTableDtos
{
	public class CreateFinalExamTimeTableDto
	{
		public string DayOfWeek { get; set; }
		public TimeOnly StartTime { get; set; }
		public TimeOnly EndTime { get; set; }
		public DateOnly Date { get; set; }
		public string Location { get; set; }
		public int AvailableCourseId { get; set; }
	}
}
