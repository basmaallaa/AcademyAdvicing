using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Models
{
    public class FinalExamTimeTable
    {
        public int Id { get; set; }
        public string DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly Date {  get; set; }
        public string Location { get; set; }
        public Coordinator UploadedBy { get; set; }
        public int UploadedById { get; set; }


		public int? AvailableCourseId { get; set; }
		public AvailableCourse AvailableCourses { get; set; }
		public List<Student> Students { get; set; }

	}
}
