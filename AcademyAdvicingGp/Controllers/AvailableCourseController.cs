using Academy.Core.Dtos;
using Academy.Core.Models;
using Academy.Core.ServicesInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AcademyAdvicingGp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailableCourseController : ControllerBase
    {
        private readonly IAvailableCourse _availableCourseService;

        public AvailableCourseController(IAvailableCourse availableCourseService)
        {
            _availableCourseService = availableCourseService;
        }

        /*[HttpPost]
        public async Task<IActionResult> CreateAvailableCourse([FromBody] AvailableCourseDto availableCourseDto)
        {
            var result = await _availableCourseService.CreateAvailableCourseAsync(availableCourseDto);
            return CreatedAtAction(nameof(GetAvailableCourseById), new { id = result.AcademicYears }, result);
        }*/
        [HttpPost]
        public async Task<IActionResult> CreateAvailableCourse([FromBody] AvailableCourseDto availableCourseDto)
        {
            // التحقق مما إذا كان الكورس مضافًا مسبقًا
            bool isAvailable = await _availableCourseService.IsCourseAvailableAsync(
                availableCourseDto.CourseId, availableCourseDto.AcademicYears, availableCourseDto.Semester);

            if (isAvailable)
            {
                return BadRequest(new { message = "This course is already available." });
            }

            // إذا لم يكن مضافًا، قم بإنشائه
            var result = await _availableCourseService.CreateAvailableCourseAsync(availableCourseDto);

            if (result == null)
            {
                return BadRequest(new { message = "Failed to create available course." });
            }

            return Ok(new { message = "Available course created successfully", id = result.CourseId });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAvailableCourse(int id, [FromBody] AvailableCourseDto updateAvailableCourseDto)
        {
            var result = await _availableCourseService.UpdateAvailableCourseAsync(id, updateAvailableCourseDto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAvailableCourseById(int id)
        {
            var result = await _availableCourseService.GetAvailableCourseByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAvailableCourses()
        {
            var result = await _availableCourseService.GetAllAvailableCoursesAsync();
            return Ok(result);
        }

        /*[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAvailableCourse(int id)
        {
            var success = await _availableCourseService.DeleteAvailableCourseAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }*/
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAvailableCourse(int id)
        {
            var success = await _availableCourseService.DeleteAvailableCourseAsync(id);
            if (!success)
                return NotFound(new { message = "The course was not found or has already been deleted!" });

            return Ok(new { message = "The course has been successfully deleted!" });
        }

    }
}

