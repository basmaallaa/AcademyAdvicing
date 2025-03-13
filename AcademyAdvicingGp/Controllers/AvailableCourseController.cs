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

        [HttpPost("Add")]
        public async Task<IActionResult> CreateAvailableCourse([FromBody] AvailableCourseDto availableCourseDto)
        {
            var result = await _availableCourseService.CreateAvailableCourseAsync(availableCourseDto);
            return CreatedAtAction(nameof(GetAvailableCourseById), new { id = result.AcademicYears }, result);
        }

        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> UpdateAvailableCourse(int id, [FromBody] AvailableCourseDto updateAvailableCourseDto)
        {
            var result = await _availableCourseService.UpdateAvailableCourseAsync(id, updateAvailableCourseDto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("GetByIdView/{id}")]
        public async Task<IActionResult> GetAvailableCourseById(int id)
        {
            var result = await _availableCourseService.GetAvailableCourseByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("GetAllForView")]
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
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAvailableCourse(int id)
        {
            var success = await _availableCourseService.DeleteAvailableCourseAsync(id);
            if (!success)
                return NotFound(new { message = "The course was not found or has already been deleted!" });

            return Ok(new { message = "The course has been successfully deleted!" });
        }

    }
}

