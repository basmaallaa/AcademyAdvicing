﻿using Academy.Core.Dtos.MaterialsDtos;
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
		Task<IEnumerable<MaterialViewDto>> GetMaterialsForStudentAsync(int studentId);
		Task<IEnumerable<MaterialViewDto>> GetMaterialsForCourseAsync(int studentId, int courseId);
		Task<IEnumerable<MaterialViewDto>> GetMaterialsUploadedByDoctorAsync(int doctorId);

	}
}
