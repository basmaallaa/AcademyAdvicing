using Academy.Core;
using Academy.Core.Enums;
using Academy.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AcademyAdvicingGp.Controllers
{
    public class ViewAvailableCoursetToDocController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ViewAvailableCoursetToDocController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }




        [HttpGet("View_Available_Course_TO_Doctor")]
        public async Task<IActionResult> GetDoctorCourses(
                    int doctorId,
                    int academicYear,
                    Semster semester)
        {
            var courses = await _unitOfWork.Repository<AvailableCourse>()
              .GetAllIncludingAsync(
                                     d => d.DoctorId == doctorId &&
                                     d.AcademicYears == academicYear &&
                                     d.Semester == semester,
                                     d => d.Course);


            if (courses == null || !courses.Any())
                return NotFound("No assigned courses found for this doctor in the specified year and semester.");

            var result = courses.Select(dac => new
            {
                CourseId = dac.Course.CourseId,
                Name = dac.Course.Name,
                Code = dac.Course.CourseCode,
                AcademicYear = dac.AcademicYears,
                Semester = dac.Semester
            });

            return Ok(result);
        }

    }
}
