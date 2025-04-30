using Academy.Core.Enums;
using Academy.Core.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Academy.Core.Dtos
{
	public class StudentDto
	{
		
		public string Name { get; set; }
        //public string UserName { get; set; }
        public string AdmissionYear { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
		public string PhoneNumber { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string Level { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Status Status { get; set; }
		public float GPA { get; set; }
		public int CompeletedHours { get; set; }

		//حاسه مالهاش لازمه نعرض معاه الكورسيز بتاعتو
		//public List<AssignedCourse> Courses { get; set; }
		
	}
}
