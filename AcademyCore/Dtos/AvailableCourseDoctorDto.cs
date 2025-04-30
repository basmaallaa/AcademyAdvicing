using Academy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Dtos
{
    public class AvailableCourseDoctorDto
    {
        public List<int> DoctorIds { get; set; }
      
        public int AcademicYears { get; set; }
        public Semster Semester { get; set; }


        public string Level { get; set; }
        

        public int CourseId { get; set; }

    }
}
