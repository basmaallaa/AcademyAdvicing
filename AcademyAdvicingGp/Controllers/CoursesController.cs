﻿using Academy.Core.Dtos;
using Academy.Core.Enums;
using Academy.Core.ServicesInterfaces.ICoursesInterface;
using Academy.Repo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademyAdvicingGp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CoursesController : ControllerBase
    {


        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpPost]
        [Authorize(Roles = "Coordinator")]
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
                // ✅ التحقق من وجود كورس بنفس التفاصيل
                var isCourseExist = await _courseService.IsCourseExistAsync(createCourseDto);
                if (isCourseExist)
                {
                    return BadRequest("This course already exists and cannot be added again.");
                }

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
        [Authorize(Roles = "Coordinator")]
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


        

            [HttpPut("{id}")]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> UpdateCourse(int id, CreateCourseDto updateCourseDto)
            {
                if (updateCourseDto == null)
                    return BadRequest("Invalid course data.");

                var updatedCourse = await _courseService.UpdateCourseAsync(id, updateCourseDto);
                if (updatedCourse == null)
                    return NotFound($"Course with ID {id} not found.");

                return Ok(updatedCourse);
            }




            [HttpGet("{id}")]
         [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> GetCourseById(int id)
            {
                var course = await _courseService.GetCourseByIdAsync(id);
                if (course == null)
                    return NotFound($"Course with ID {id} not found.");

                return Ok(course);
            }

            [HttpGet]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> GetAllCourses()
            {
                var courses = await _courseService.GetAllCoursesAsync();
                return Ok(courses);
            }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var deleted = await _courseService.DeleteCourseAsync(id);
            if (!deleted)
                return NotFound($"Course with ID {id} not found.");

            return Ok($"Course with ID {id} deleted successfully.");
        }
    }

    } 






