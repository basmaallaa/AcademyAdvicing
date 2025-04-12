using Academy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Dtos
{
    public class AvailableAssignCourse
    {
        public int AvailableCourseId { get; set; }
        public string CourseName { get; set; }
        public int  CourseId { get; set; }
        public string CourseCode { get; set; }
        public int AcademicYear { get; set; }
        public Semster Semester { get; set; }
    }
}
