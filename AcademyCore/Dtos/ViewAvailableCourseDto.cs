using Academy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Academy.Core.Dtos
{
    public class ViewAvailableCourseDto
    {
            public int Id { get; set; }  // ID الخاص بـ Available Course
            public int AcademicYears { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Semster Semester { get; set; }

            // بيانات الكورس المختار
            public int CourseId { get; set; }
            public string CourseName { get; set; }
            public string CourseCode { get; set; }
            public int CreditHours { get; set; }
            



    }
}
