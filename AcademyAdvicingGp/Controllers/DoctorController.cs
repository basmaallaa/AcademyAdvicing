using Academy.Core.Dtos;
using Academy.Core.Models;
using Academy.Core.ServicesInterfaces;
using Academy.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace AcademyAdvicingGp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            var result = await _doctorService.GetAllDoctorsAsync();

            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorById(int? id)
        {
            if (id is null) return BadRequest("Invalid id !!");

            var result = await _doctorService.GetDoctorByIdAsync(id.Value);

            if (result is null) return NotFound($"The Doctor with Id: {id} not found :(  ");
            return Ok(result);
        }



        [HttpPost]
        public async Task<IActionResult> AddDoctor([FromBody] DoctorDto doctorDto)
        {
            if (doctorDto == null)
                return BadRequest("Doctor data is required.");

            var addedDoctor = await _doctorService.AddDoctorAsync(doctorDto);
            return CreatedAtAction(nameof(GetAllDoctors), addedDoctor);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] DoctorDtoID doctorDtoId)
        {
            if (doctorDtoId == null || id != doctorDtoId.Id)
                return BadRequest("Invalid doctor data or ID mismatch.");

            var existingDoctor = await _doctorService.GetDoctorByIdAsync(id);
            if (existingDoctor == null)
                return NotFound($"Doctor with ID {id} not found.");

            await _doctorService.UpdateDoctorAsync(doctorDtoId);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var existingDoctor = await _doctorService.GetDoctorByIdAsync(id);
            if (existingDoctor == null)
                return NotFound($"Doctor with ID {id} not found.");

            await _doctorService.DeleteDoctorAsync(id);
            return Ok();
        }


        [HttpGet("search")]
        public async Task<IActionResult> SearchDoctors([FromQuery] string searchTerm)
        {
            var result = await _doctorService.SearchDoctorsAsync(searchTerm);

            if (result.Any())
            {
                return Ok(result);
            }

            return NotFound("No doctors found matching the search term.");
        }
    }
}
