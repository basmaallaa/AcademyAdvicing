using Academy.Core;
using Academy.Core.Dtos;
using Academy.Core.Enums;
using Academy.Core.Models;
using Academy.Core.ServicesInterfaces;
using Academy.Repo.Data;
using Academy.Services.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq;


namespace AcademyAdvicingGp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailableCourseController : ControllerBase
    {
        private readonly IAvailableCourse _availableCourseService;
        private readonly IDoctorCourseService _doctorCourseService;
        private readonly IDoctorService _doctorService;
        private readonly IUnitOfWork _unitOfWork;
        

        private readonly IMapper _mapper;
        private readonly AcademyContext _academyDbContext;

        public AvailableCourseController(IAvailableCourse availableCourseService, IDoctorCourseService doctorCourseService , IDoctorService doctorService, IUnitOfWork unitOfWork, IMapper mapper,AcademyContext academyContext)
        {
            _availableCourseService = availableCourseService;
            _doctorCourseService = doctorCourseService;
            _doctorService = doctorService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _academyDbContext=academyContext;
        }





        

        [HttpPost("AddAvailableCourseWithDoctors")]
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> AssignDoctorsToAvailableCourse([FromBody] AvailableCourseDoctorDto dto)
        {
            if (dto == null || dto.DoctorIds == null || !dto.DoctorIds.Any())
                return BadRequest("Invalid data.");

            // التأكد من أن الكورس موجود فعلاً
            var courseExists = await _unitOfWork.Repository<Course>().AnyAsync(c => c.CourseId == dto.CourseId);
            if (!courseExists)
                return BadRequest($"Course with ID {dto.CourseId} does not exist.");

            var alreadyAssignedDoctors = new List<int>();
            var newAssignments = new List<AvailableCourse>();

            foreach (var doctorId in dto.DoctorIds)
            {
                // هل فيه بالفعل AvailableCourse لنفس الدكتور والكورس والسنة والترم والمستوى؟
                var alreadyLinked = await _unitOfWork.Repository<AvailableCourse>().AnyAsync(ac =>
                    ac.DoctorId == doctorId &&
                    ac.CourseId == dto.CourseId &&
                    ac.AcademicYears == dto.AcademicYears &&
                    ac.Semester == dto.Semester &&
                    ac.Level == dto.Level
                );

                if (alreadyLinked)
                {
                    alreadyAssignedDoctors.Add(doctorId);
                }
                else
                {
                    newAssignments.Add(new AvailableCourse
                    {
                        DoctorId = doctorId,
                        CourseId = dto.CourseId,
                        AcademicYears = dto.AcademicYears,
                        Semester = dto.Semester,
                        Level = dto.Level
                    });
                }
            }

            // أضف التعيينات الجديدة
            if (newAssignments.Any())
            {
                foreach (var assignment in newAssignments)
                {
                    await _unitOfWork.Repository<AvailableCourse>().AddAsync(assignment);
                }

                await _unitOfWork.CompleteAsync();
            }

            // لو فيه دكاترة متسجلين قبل كده
            if (alreadyAssignedDoctors.Any())
            {
                return Ok(new
                {
                    message = "Some doctors were already assigned to this course for the same semester, academic year, and level.",
                    duplicateDoctorIds = alreadyAssignedDoctors
                });
            }

            return Ok("Doctors assigned to the available course successfully.");
        }



        [HttpPut("Edit/{id}")]
        
        public async Task<IActionResult> UpdateAvailableCourse(int id, [FromBody] AvailableCourseDoctorDto updateAvailableCourseDto)
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
        [Authorize(Roles = "Coordinator")]
        public async Task<IActionResult> GetAllAvailableCourses()
        {
            var result = await _availableCourseService.GetAllAvailableCoursesAsync();
            return Ok(result);
        }



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

