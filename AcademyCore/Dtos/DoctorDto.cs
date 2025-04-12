using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Dtos
{
    public class DoctorDto
    {
       
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string ImagePath { get; set; }
        public int AssignedById { get; set; }
    }
}
