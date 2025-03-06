using Academy.Core.Dtos;
using Academy.Core.Enums;
using Academy.Core.ServicesInterfaces.ICoursesInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AcademyAdvicingGp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
  

            private readonly ICourseService _courseService;

            public CoursesController(ICourseService courseService)
            {
                _courseService = courseService;
            }

        // ✅ إضافة كورس جديد
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto createCourseDto)
        {
            if (createCourseDto == null)
                return BadRequest("Invalid course data.");

            // ✅ التحقق من صحة القيم المدخلة في الـ Enum
            if (!Enum.IsDefined(typeof(courseCategory), createCourseDto.category) ||
                !Enum.IsDefined(typeof(courseType), createCourseDto.type))
            {
                return BadRequest("Invalid category or type value.");
            }

            try
            {
                var createdCourse = await _courseService.CreateCourseAsync(createCourseDto);
                return Ok(createdCourse);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
                return StatusCode(500, $"Error creating course: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCourses(
    [FromQuery] string? name)
    /*[FromQuery] string? courseCode,
    [FromQuery] int? creditHours,
    [FromQuery] courseType? type,
    [FromQuery] courseCategory? category)*/
        {
            var result = await _courseService.SearchCoursesAsync(name/*, courseCode, creditHours, type, category*/);
            return Ok(result);
        }

    }
}




