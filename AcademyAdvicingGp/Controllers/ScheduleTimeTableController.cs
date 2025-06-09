using Academy.Core.Dtos.ScheduleDtos;
using Academy.Core.Models;
using Academy.Core.ServicesInterfaces;
using Academy.Repo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcademyAdvicingGp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ScheduleTimeTableController : ControllerBase
	{
		private readonly IScheduleTimeTableService _scheduleTimeTableService;
		private readonly AcademyContext _academyContext;

		public ScheduleTimeTableController(IScheduleTimeTableService service , AcademyContext academyContext)
		{
			_scheduleTimeTableService = service;
			_academyContext = academyContext;
		}

		[HttpGet]
		[Authorize(Roles = "Coordinator")]
		public async Task<IActionResult> GetAll()
		{
			var result = await _scheduleTimeTableService.GetAllAsync();
			return Ok(result);
		}

		[HttpGet("{id}")]
		[Authorize(Roles = "Coordinator")]
		public async Task<IActionResult> Get(int id)
		{
			var result = await _scheduleTimeTableService.GetByIdAsync(id);
			return result == null ? NotFound() : Ok(result);
		}

		//[HttpPost]
		//public async Task<IActionResult> Create(CreateScheduleTimeTableDto dto)
		//{
		//	var result = await _scheduleTimeTableService.AddAsync(dto);
		//	return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
		//}

		[HttpPost]
		[Authorize(Roles = "Coordinator")]
		public async Task<ActionResult> Create([FromBody] CreateScheduleTimeTableDto dto)
		{
			try
			{
				var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
				if (string.IsNullOrEmpty(userEmail))
					return Unauthorized("Email not found in token.");

				// 2. Get coordinator by email
				var coordinator = await _academyContext.Coordinator
					.FirstOrDefaultAsync(c => c.Email == userEmail);

				if (coordinator == null)
					return NotFound("Coordinator profile not found.");

				var result = await _scheduleTimeTableService.AddAsync(dto , coordinator.Id);
				//return Ok(result);
				return Ok("Course Time Added Successfully ;)");
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				// optional: log the error
				return StatusCode(500, new { message = "An unexpected error occurred." });
			}



		}


		[HttpPut("update/{id}")]
		[Authorize(Roles = "Coordinator")]
		public async Task<IActionResult> UpdateAsync(int id, EditScheduleTimeTableDto dto)
		{
			try
			{
				var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
				if (string.IsNullOrEmpty(userEmail))
					return Unauthorized("Email not found in token.");

				// 2. Get coordinator by email
				var coordinator = await _academyContext.Coordinator
								.FirstOrDefaultAsync(c => c.Email == userEmail);

				if (coordinator == null)
					return NotFound("Coordinator profile not found.");

				await _scheduleTimeTableService.UpdateScheduleTimeTableAsync(id, dto, coordinator.Id);
				return Ok("Updated Successfully");
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception)
			{
				return StatusCode(500, new { message = "An unexpected error occurred." });
			}
		}


		[HttpDelete("{id}")]
		[Authorize(Roles = "Coordinator")]
		public async Task<IActionResult> Delete(int id)
		{
			var existingScheduleTimeTable = await _scheduleTimeTableService.GetByIdAsync(id);
			if (existingScheduleTimeTable == null)
				return NotFound($"Time Table with ID {id} not found.");

			await _scheduleTimeTableService.DeleteAsync(id);
			return Ok("Schedule Deleted Successfully");
		}


		[HttpPost("bulk")]
		[Authorize(Roles = "Coordinator")]
		public async Task<IActionResult> AddBulkSchedule([FromBody] CreateFullScheduleDto dto)
		{
			try
			{
				var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
				if (string.IsNullOrEmpty(userEmail))
					return Unauthorized("Email not found in token.");

				// 2. Get coordinator by email
				var coordinator = await _academyContext.Coordinator
					.FirstOrDefaultAsync(c => c.Email == userEmail);

				if (coordinator == null)
					return NotFound("Coordinator profile not found.");


				await _scheduleTimeTableService.AddBulkScheduleAsync(dto,coordinator.Id);
				return Ok(new { message = "Bulk schedule added successfully." });
			}
			catch (Exception ex)
			{
				return BadRequest(new { error = ex.Message });
			}
		}


		//[HttpGet("student/{studentId}")]
		//public async Task<IActionResult> GetScheduleForStudent(int studentId)
		//{
		//	var schedule = await _scheduleTimeTableService.GetStudentSchedule(studentId);

		//	if (schedule == null || !schedule.Any())
		//		return NotFound("No schedule found for this student.");

		//	return Ok(schedule);
		//}

		[HttpGet("my-schedule")]
		[Authorize(Roles = "Student")]
		public async Task<IActionResult> GetMySchedule()
		{
			var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
			if (string.IsNullOrEmpty(userEmail))
				return Unauthorized("Email not found in token.");

			// جلب الطالب بناءً على الإيميل
			var student = await _academyContext.Students
					.FirstOrDefaultAsync(s => s.Email == userEmail);

			if (student == null)
				return NotFound("Student profile not found.");

			// استدعاء الخدمة باستخدام studentId
			var schedule = await _scheduleTimeTableService.GetStudentSchedule(student.Id);

			if (schedule == null || !schedule.Any())
				return NotFound("No schedule found for this student.");

			return Ok(schedule);
		}


		[HttpGet("download-schedule")]
		[Authorize(Roles = "Student")]
		public async Task<IActionResult> DownloadMySchedule()
		{
			var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
			if (string.IsNullOrEmpty(userEmail))
				return Unauthorized("Email not found in token.");

			var student = await _academyContext.Students
				.FirstOrDefaultAsync(s => s.Email == userEmail);

			if (student == null)
				return NotFound("Student not found.");

			var schedule = await _scheduleTimeTableService.GetStudentSchedule(student.Id);
			if (schedule == null || !schedule.Any())
				return NotFound("No schedule found.");

			var pdfBytes = _scheduleTimeTableService.GenerateStudentSchedulePdf(schedule,student.Name,student.Level);
			return File(pdfBytes, "application/pdf", "MySchedule.pdf");
		}
	}
}
