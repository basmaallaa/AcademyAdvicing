using Academy.Core.Dtos.MaterialsDtos;
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
	public class MaterialController : ControllerBase
	{
		private readonly IMaterialService _materialService;
		private readonly AcademyContext _academyContext;

		public MaterialController(IMaterialService materialService , AcademyContext academyContext)
		{
			_materialService = materialService;
			_academyContext = academyContext;
		}

		//upload pdf
		//[HttpPost("upload")]
		//[Authorize(Roles = "Doctor")]
		//public async Task<IActionResult> UploadMaterial([FromForm] MaterialDto materialDto, [FromForm] IFormFile file)
		//{
		//	try
		//	{
		//		var result = await _materialService.AddAsync(materialDto, file);
		//		return Ok("Material Uploaded successfully.");
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.WriteLine($"Upload Error: {ex.Message}"); // 3lshan afhm el error fen
		//		return BadRequest(new { message = "An error occurred while uploading the file. Please try again." });
		//	}
		//}
		[HttpPost("upload")]
		[Authorize(Roles = "Doctor")]
		public async Task<IActionResult> UploadMaterial([FromForm] CreateMaterialDto materialDto, IFormFile file)
		{
			try
			{
				var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
				if (string.IsNullOrEmpty(userEmail))
					return Unauthorized("Email not found in token.");

				var doctor = await _academyContext.Doctors.FirstOrDefaultAsync(d => d.Email == userEmail);
				if (doctor == null)
					return NotFound("Doctor profile not found.");

				//materialDto.UploadedById = doctor.Id;
				var material = new MaterialDto
				{
					Title = materialDto.Title,
					CourseId = materialDto.CourseId,
					UploadedById = doctor.Id,
					
				};


				var result = await _materialService.AddAsync(material, file);

				return Ok("Material Uploaded successfully.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Upload Error: {ex.Message}"); // علشان نعرف الغلط فين
				return BadRequest(new { message = "An error occurred while uploading the file. Please try again." });
			}
		}




		//Get All materialss
		[HttpGet]
		[Authorize(Roles = "Doctor")]
		public async Task<IActionResult> GetAllMaterials()
		{
			var materials = await _materialService.GetAllAsync();
			return Ok(materials);
		}



		// Delete Material
		[HttpDelete("{id}")]
		[Authorize(Roles = "Doctor")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				await _materialService.DeleteAsync(id);
				return Ok("Material deleted successfully.");
			}
			catch (FileNotFoundException)
			{
				return NotFound("The file does not exist.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error deleting material: {ex.Message}");
				return Ok("The file does not exist.");
			}
		}



		//Download the material
		[HttpGet("download/{id}")]
		public async Task<IActionResult> DownloadMaterial(int id)
		{
			try
			{
				var (fileBytes, fileName) = await _materialService.DownloadMaterialAsync(id);
				return File(fileBytes, "application/pdf", fileName);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { error = ex.Message });
			}
			catch (FileNotFoundException ex)
			{
				return NotFound(new { error = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
			}
		}

		[HttpGet("student/materials")]
		[Authorize(Roles = "Student")]
		public async Task<IActionResult> GetMaterialsForStudent()
		{
			try
			{
				var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
				if (string.IsNullOrEmpty(userEmail))
					return Unauthorized("Email not found in token.");

				// نجيب الطالب بالإيميل
				var student = await _academyContext.Students
					.FirstOrDefaultAsync(s => s.Email == userEmail);

				if (student == null)
					return NotFound("Student profile not found.");

				var materials = await _materialService.GetMaterialsForStudentAsync(student.Id);
				return Ok(materials);
			}
			catch (Exception ex)
			{
				return BadRequest(new { error = ex.Message });
			}
		}

		[HttpGet("student/course/{courseId}/materials")]
		[Authorize(Roles = "Student")]
		public async Task<IActionResult> GetMaterialsForStudentCourse(int courseId)
		{
			try
			{
				var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
				if (string.IsNullOrEmpty(userEmail))
					return Unauthorized("Email not found in token.");

				var student = await _academyContext.Students
					.FirstOrDefaultAsync(s => s.Email == userEmail);

				if (student == null)
					return NotFound("Student profile not found.");

				var materials = await _materialService.GetMaterialsForCourseAsync(student.Id, courseId);
				return Ok(materials);
			}
			catch (Exception ex)
			{
				return BadRequest(new { error = ex.Message });
			}
		}

		[HttpGet("doctor/materials")]
		[Authorize(Roles = "Doctor")]
		public async Task<IActionResult> GetMaterialsUploadedByDoctor()
		{
			try
			{
				var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
				if (string.IsNullOrEmpty(userEmail))
					return Unauthorized("Email not found in token.");

				var doctor = await _academyContext.Doctors
					.FirstOrDefaultAsync(d => d.Email == userEmail);

				if (doctor == null)
					return NotFound("Doctor profile not found.");

				var materials = await _materialService.GetMaterialsUploadedByDoctorAsync(doctor.Id);
				return Ok(materials);
			}
			catch (Exception ex)
			{
				return BadRequest(new { error = ex.Message });
			}
		}



		#region Get By Id End point 3lshan lw ehtgnaha
		//[HttpGet("{id}")]
		//public async Task<IActionResult> GetById(int id)
		//{
		//	var materialDto = await _materialService.GetByIdAsync(id);
		//	if (materialDto == null)
		//	{
		//		return NotFound();
		//	}
		//	else
		//	{
		//		return Ok(materialDto);
		//	}
		//}


		#endregion

	}
}
