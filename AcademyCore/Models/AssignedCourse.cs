using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Models
{
    public class AssignedCourse
    {
        public int  StudentId { get; set; } //FK
        public int  CourseId { get; set; } //FK

        public float ClassWorkScore { get; set; }
        public float PracticalScore { get; set; }
        public float FinalScore { get; set; }
        public string Grade { get; set; }

        public Student Student { get; set; }
        public Course Course { get; set; }

    }
}
