using Academy.Core.Dtos.ScheduleDtos;
using Academy.Core.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AcademyAdvicingGp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ScheduleTimeTableController : ControllerBase
	{
		private readonly IScheduleTimeTableService _scheduleTimeTableService;

		public ScheduleTimeTableController(IScheduleTimeTableService service)
		{
			_scheduleTimeTableService = service;
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
		public async Task<ActionResult<ScheduleTimeTableDto>> Create([FromBody] CreateScheduleTimeTableDto dto)
		{
			try
			{
				var result = await _scheduleTimeTableService.AddAsync(dto);
				return Ok(result);
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
		public async Task<IActionResult> UpdateAsync(int id, CreateScheduleTimeTableDto dto)
		{
			try
			{
				await _scheduleTimeTableService.UpdateScheduleTimeTableAsync(id, dto);
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
			return Ok();
		}


		[HttpPost("bulk")]
		[Authorize(Roles = "Coordinator")]
		public async Task<IActionResult> AddBulkSchedule([FromBody] CreateFullScheduleDto dto)
		{
			try
			{
				await _scheduleTimeTableService.AddBulkScheduleAsync(dto);
				return Ok(new { message = "Bulk schedule added successfully." });
			}
			catch (Exception ex)
			{
				return BadRequest(new { error = ex.Message });
			}
		}


	}
}
