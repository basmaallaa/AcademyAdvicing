using Academy.Core.Dtos;
using Academy.Core.ServicesInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AcademyAdvicingGp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MaterialController : ControllerBase
	{
		private readonly IMaterialService _materialService;

		public MaterialController(IMaterialService materialService)
		{
			_materialService = materialService;
		}

		//upload pdf
		[HttpPost("upload")]
		public async Task<IActionResult> UploadMaterial([FromForm] MaterialDto materialDto, [FromForm] IFormFile file)
		{
			try
			{
				var result = await _materialService.AddAsync(materialDto, file);
				return Ok("Material Uploaded successfully.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Upload Error: {ex.Message}"); // 3lshan afhm el error fen
				return BadRequest(new { message = "An error occurred while uploading the file. Please try again." });
			}
		}



		//Get All materialss
		[HttpGet]
		public async Task<IActionResult> GetAllMaterials()
		{
			var materials = await _materialService.GetAllAsync();
			return Ok(materials);
		}



		// Delete Material
		[HttpDelete("{id}")]
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
