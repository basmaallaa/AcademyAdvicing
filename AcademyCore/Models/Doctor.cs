using Academy.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Models
{
    public class Doctor : Person
    {
        
        public Coordinator AssignedBy { get; set; }
        public int AssignedById { get; set; }
        public List<Material> Materials { get; set; }
        public List<DoctorCourses> Courses { get; set; }


    }
}
