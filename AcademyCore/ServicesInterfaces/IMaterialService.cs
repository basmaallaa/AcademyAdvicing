using Academy.Core.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.ServicesInterfaces
{
	public interface IMaterialService
	{
		Task<MaterialDto> GetByIdAsync(int id);
		Task<MaterialDto> AddAsync(MaterialDto materialDto, IFormFile file);
		Task DeleteAsync(int id);
		Task<IEnumerable<MaterialDto>> GetAllAsync();
		Task<(byte[], string)> DownloadMaterialAsync(int materialId);
		Task<IEnumerable<MaterialDto>> GetMaterialsForStudentAsync(int studentId);

	}
}
