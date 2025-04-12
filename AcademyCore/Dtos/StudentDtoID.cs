using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Dtos
{
    public class StudentDtoID
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ImagePath { get; set; }
        public string Level { get; set; }
        public string Status { get; set; }
        public float GPA { get; set; }
        public int CompeletedHours { get; set; }
        public string Token { get; set; }

    }
}
