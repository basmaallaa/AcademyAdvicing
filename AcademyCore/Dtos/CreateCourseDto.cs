using Academy.Core.Enums;
using Academy.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Academy.Core.Dtos
{
    public class CreateCourseDto
    {

        public string Name { get; set; }
        public string CourseCode { get; set; }
        [Required]
        [Range(2, 3, ErrorMessage = "CreditHours must be either 2 or 3.")]
        public int CreditHours { get; set; }
        public float Credit { get; set; }

        //public string? prerequisite { get; set; }
        public int? PrerequisiteCourseId { get; set; }


        [JsonConverter(typeof(JsonStringEnumConverter))]
        public courseCategory category { get; set; }


        [JsonConverter(typeof(JsonStringEnumConverter))]
        public courseType type { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CreditHours != 2 && CreditHours != 3)
            {
                yield return new ValidationResult("CreditHours must be either 2 or 3.", new[] { nameof(CreditHours) });
            }



        }
    }
}
