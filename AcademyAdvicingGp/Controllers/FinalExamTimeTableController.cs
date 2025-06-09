using Academy.Core.Dtos.FinalTimeTableDtos;
using Academy.Core.Dtos.ScheduleDtos;
using Academy.Core.ServicesInterfaces;
using Academy.Repo.Data;
using Academy.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcademyAdvicingGp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FinalExamTimeTableController : ControllerBase
	{
		private readonly IFinalExamTimeTableService _finalExamTimeTableService;
		private readonly AcademyContext _academyContext;

		public FinalExamTimeTableController(IFinalExamTimeTableService finalExamTimeTableService , AcademyContext academyContext)
        {
			_finalExamTimeTableService = finalExamTimeTableService;
			_academyContext = academyContext;
		}

		[HttpGet]
		[Authorize(Roles = "Coordinator")]
		public async Task<IActionResult> GetAll()
		{
			var result = await _finalExamTimeTableService.GetAllAsync();
			return Ok(result);
		}

		[HttpGet("{id}")]
		[Authorize(Roles = "Coordinator")]
		public async Task<IActionResult> Get(int id)
		{
			var result = await _finalExamTimeTableService.GetByIdAsync(id);
			return result == null ? NotFound() : Ok(result);
		}


		[HttpPost]
		[Authorize(Roles = "Coordinator")]
		public async Task<ActionResult> Create([FromBody] CreateFinalExamTimeTableDto dto)
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

				var result = await _finalExamTimeTableService.AddAsync(dto, coordinator.Id);
				//return Ok(result);
				return Ok("Final Time Added Successfully ;)");
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(500, new { message = "An unexpected error occurred." });
			}
		}


		[HttpPut("update/{id}")]
		[Authorize(Roles = "Coordinator")]
		public async Task<IActionResult> UpdateAsync(int id, EditFinalExamTimeTableDto dto)
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

				await _finalExamTimeTableService.UpdateFinalTimeTableAsync(id, dto, coordinator.Id);
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
			var existingScheduleTimeTable = await _finalExamTimeTableService.GetByIdAsync(id);
			if (existingScheduleTimeTable == null)
				return NotFound($"Time Table with ID {id} not found.");

			await _finalExamTimeTableService.DeleteAsync(id);
			return Ok("Schedule Deleted Successfully");
		}

		[HttpPost("bulk")]
		[Authorize(Roles = "Coordinator")]
		public async Task<IActionResult> AddBulkSchedule([FromBody] CreateFullFinalTimeTableDto dto)
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


				await _finalExamTimeTableService.AddBulkFinalTimeTableAsync(dto,coordinator.Id);
				return Ok(new { message = "Bulk schedule added successfully." });
			}
			catch (Exception ex)
			{
				return BadRequest(new { error = ex.Message });
			}
		}


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
			var schedule = await _finalExamTimeTableService.GetStudentFinalExamSchedule(student.Id);

			if (schedule == null || !schedule.Any())
				return NotFound("No final exam time table found for this student.");

			return Ok(schedule);
		}

		[HttpGet("download-schedule")]
		[Authorize(Roles = "Student")]
		public async Task<IActionResult> DownloadMyFinalTimeTable()
		{
			var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
			if (string.IsNullOrEmpty(userEmail))
				return Unauthorized("Email not found in token.");

			var student = await _academyContext.Students
				.FirstOrDefaultAsync(s => s.Email == userEmail);

			if (student == null)
				return NotFound("Student not found.");

			var schedule = await _finalExamTimeTableService.GetStudentFinalExamSchedule(student.Id);
			if (schedule == null || !schedule.Any())
				return NotFound("No schedule found.");

			var pdfBytes = _finalExamTimeTableService.GenerateFinalExamPdf(schedule, student.Name, student.Level);
			return File(pdfBytes, "application/pdf", "MySchedule.pdf");
		}
	}
}
