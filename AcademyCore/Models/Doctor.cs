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
        public List<Material> Materials { get; set; }
        public List<AvailableCourse> Courses { get; set; }



    }
}
