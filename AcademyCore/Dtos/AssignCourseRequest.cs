using Academy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Dtos
{
    public class AssignCourseRequest
    {
        public int StudentId { get; set; }
        //public int CourseId { get; set; }
        //public int AcademicYear { get; set; }
        //public Semster Semester { get; set; }

        public List<int> AvailableCourseIds { get; set; }
    }
}
