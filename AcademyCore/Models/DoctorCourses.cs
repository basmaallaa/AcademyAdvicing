using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Models
{
	public class DoctorCourses
	{
		public int DoctorId { get; set; } //FK
		public int CourseId { get; set; } //FK
		public Doctor Doctor { get; set; }
		public Course Course { get; set; }

	}
}
