using Academy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Models
{
    public class AvailableCourse
    {
        public int Id { get; set; }


        public int AcademicYears { get; set; }

        public Semster Semester { get; set; }

        public int CourseId { get; set; }

        public Course Course { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        //public List<DoctorAvailableCourses> Doctors { get; set; }

    }
}

