using Academy.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Dtos
{
	public class StudentDto
	{
		
		public string Name { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string PhoneNumber { get; set; }
		public string ImagePath { get; set; }
		public string Level { get; set; }
		public string Status { get; set; }
		public float GPA { get; set; }
		public int CompeletedHours { get; set; }

		//حاسه مالهاش لازمه نعرض معاه الكورسيز بتاعتو
		//public List<AssignedCourse> Courses { get; set; }
		
	}
}
