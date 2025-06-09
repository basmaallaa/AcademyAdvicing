using Academy.Core.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Dtos
{
    public class TranscriptDto
    {
        public string StudentName { get; set; }
        public float GPA { get; set; }
        public int CompletedHours { get; set; }
        public string Status { get; set; }
        public IFormFile? ImageFile { get; set; }
        public float? TotalGrades { get; set; }
        public Levels Levels { get; set; }
        public List<CourseGradeDto> Courses { get; set; }
    }
}
