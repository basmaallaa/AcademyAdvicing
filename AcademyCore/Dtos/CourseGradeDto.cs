using Academy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Dtos
{
    public class CourseGradeDto
    {
        public string Code { get; set; }
        public string CourseName { get; set; }
        public int Hours { get; set; }
        public courseCategory Category { get; set; }
        public string GradeLetter { get; set; }
        public float? TotalGrades { get; set; } // ده اللي هنعبيه من AssignedCourse
    }

}
