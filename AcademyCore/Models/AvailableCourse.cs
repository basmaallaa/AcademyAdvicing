using Academy.Core.Dtos;
using Academy.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Academy.Core.Models
{
    public class AvailableCourse
    {
        [Key]
        public int Id { get; set; }


        public string AcademicYears { get; set; }

        public Semster Semester { get; set; }

        public Levels? Level { get; set; }
        public int CourseId { get; set; }

        public Course Course { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
       

        public List<ScheduleTimeTable> ScheduleTimeTables { get; set; }

	}
}

