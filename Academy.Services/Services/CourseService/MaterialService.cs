using Academy.Core;
using Academy.Core.Dtos;
using Academy.Core.Models;
using Academy.Core.ServicesInterfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Services.Services.CourseService
{
	public class MaterialService : IMaterialService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public MaterialService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<MaterialDto> AddAsync(MaterialDto materialDto, IFormFile file)
		{
			if (file == null || file.Length == 0)
			{
				throw new Exception("Invalid file");
			}

			var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedMaterials");

			if (!Directory.Exists(uploadsPath))
			{
				Directory.CreateDirectory(uploadsPath);
			}


			var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
			var filePath = Path.Combine(uploadsPath, uniqueFileName);


			//var material = _mapper.Map<Material>(materialDto);
			//material.FilePath = Path.Combine("uploads", uniqueFileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}


			var material = new Material
			{
				Title = materialDto.Title,
				UploadedById = materialDto.UploadedById,
				FilePath = filePath
			};
			await _unitOfWork.Repository<Material>().AddAsync(material);
			await _unitOfWork.CompleteAsync();

			return _mapper.Map<MaterialDto>(material);
		}

		public async Task DeleteAsync(int id)
		{
			var material = await _unitOfWork.Repository<Material>().GetAsync(id); 
			if (material == null)
				throw new Exception("Material not found.");

			string fullPath = Path.Combine(Directory.GetCurrentDirectory(), material.FilePath);
			if (File.Exists(fullPath))
				File.Delete(fullPath);
			else throw new FileNotFoundException($"File not found: {fullPath}");

			_unitOfWork.Repository<Material>().Delete(material);
			await _unitOfWork.CompleteAsync();

		}

		public async Task<(byte[], string)> DownloadMaterialAsync(int materialId)
		{
			var material = await _unitOfWork.Repository<Material>().GetAsync(materialId);

			if (material == null)
			{
				throw new KeyNotFoundException("Material not found.");
			}

			if (!File.Exists(material.FilePath))
			{
				throw new FileNotFoundException("File not found on the server.");
			}

			var fileBytes = await File.ReadAllBytesAsync(material.FilePath);
			return (fileBytes, material.Title);
		}

		public async Task<IEnumerable<MaterialDto>> GetAllAsync()
		{
			var materials = await _unitOfWork.Repository<Material>().GetAllAsync();
			return _mapper.Map<IEnumerable<MaterialDto>>(materials);
		}

		public async Task<MaterialDto> GetByIdAsync(int id)
		{
			var material = await _unitOfWork.Repository<Material>().GetAsync(id);
			if (material == null) return null;

			return _mapper.Map<MaterialDto>(material);
		}
	}
}
