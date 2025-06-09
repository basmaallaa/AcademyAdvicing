using Academy.Core.Enums;
using Academy.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Models
{
    public class Student : Person
    {
        
        public Levels Level { get; set; }
        public Status Status { get; set; }
        public float GPA { get; set; }
        public int CompeletedHours { get; set; }
        public List<AssignedCourse> Courses { get; set; }
        public StudentAffair ManageBy{ get; set; }
        public int? ManageById { get; set; }

        public String AdmissionYear { get; set; }


    }
}
