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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CreateCourseDto updateCourseDto)
        {
            if (updateCourseDto == null)
                return BadRequest("Invalid course data.");

            var updatedCourse = await _courseService.UpdateCourseAsync(id, updateCourseDto);
            if (updatedCourse == null)
                return NotFound($"Course with ID {id} not found.");

            return Ok(updatedCourse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound($"Course with ID {id} not found.");

            return Ok(course);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        /*[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var deleted = await _courseService.DeleteCourseAsync(id);
            if (!deleted)
                return NotFound($"Course with ID {id} not found.");

            return Ok($"Course with ID {id} deleted successfully.");
        }*/
    }

}
    
    




