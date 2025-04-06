using Academy.Core.Enums;
using Academy.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Academy.Core.Dtos
{
    public  class AvailableCourseDto
    {
        public int  AcademicYears { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Semster Semester { get; set; }
        public int CourseId { get; set; }

   
    }
}
