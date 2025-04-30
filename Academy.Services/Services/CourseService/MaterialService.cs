using Academy.Core;
using Academy.Core.Dtos.MaterialsDtos;
using Academy.Core.Models;
using Academy.Core.ServicesInterfaces;
using Academy.Repo.Data;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
		private readonly AcademyContext _academyContext;

		public MaterialService(IUnitOfWork unitOfWork, IMapper mapper, AcademyContext academyContext)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_academyContext = academyContext;
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
				CourseId = materialDto.CourseId,
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


		//public async Task<IEnumerable<MaterialDto>> GetMaterialsForStudentAsync(int studentId)
		//{
		//	// 1. هات كل الـ materials
		//	var allMaterials =  await _unitOfWork.Repository<Material>().GetAllAsync();

		//	// 2. هات كل الـ AssignedCourses
		//	var allAssignedCourses = await _unitOfWork.Repository<AssignedCourse>().GetAllAsync();

		//	// 3. فلتر الكورسات اللي متسجلة للطالب
		//	var studentCourseIds = new List<int>();


		//	foreach (var ac in allAssignedCourses)
		//	{
		//		if (ac.StudentId == studentId)
		//		{
		//			studentCourseIds.Add(ac.CourseId);
		//		}
		//	}

		//	// 4. هات كل المواد اللي تبع الكورسات دي
		//	var allMaterialsEntities = await _unitOfWork.Repository<Material>().GetAllAsync();
		//	var filteredMaterials = new List<Material>();

		//	foreach (var material in allMaterialsEntities)
		//	{
		//		if (studentCourseIds.Contains(material.CourseId)) // لازم تكون عندك CourseId في Material
		//		{
		//			filteredMaterials.Add(material);
		//		}
		//	}

		//	return _mapper.Map<IEnumerable<MaterialDto>>(filteredMaterials);
		//}


		//public async Task<IEnumerable<MaterialDto>> GetMaterialsForCourseAsync(int studentId, int courseId)
		//{
		//	var allAssignedCourses = await _unitOfWork.Repository<AssignedCourse>().GetAllAsync();

		//	var isAssigned = allAssignedCourses.Any(ac => ac.StudentId == studentId && ac.CourseId == courseId);

		//	if (!isAssigned)
		//	{
		//		throw new Exception("This student is not assigned to the specified course.");
		//	}

		//	var allMaterials = await _unitOfWork.Repository<Material>().GetAllAsync();

		//	var courseMaterials = allMaterials.Where(m => m.CourseId == courseId);

		//	return _mapper.Map<IEnumerable<MaterialDto>>(courseMaterials);
		//}
		//public async Task<IEnumerable<MaterialViewDto>> GetMaterialsForStudentAsync(int studentId)
		//{
		//	var allMaterials = await _unitOfWork.Repository<Material>().GetAllAsync();
		//	var allAssignedCourses = await _unitOfWork.Repository<AssignedCourse>().GetAllAsync();

		//	var studentCourseIds = allAssignedCourses
		//		.Where(ac => ac.StudentId == studentId)
		//		.Select(ac => ac.CourseId)
		//		.ToList();

		//	var filteredMaterials = allMaterials
		//		.Where(m => studentCourseIds.Contains(m.CourseId))
		//		.ToList();

		//	return _mapper.Map<IEnumerable<MaterialViewDto>>(filteredMaterials);
		//}
		public async Task<IEnumerable<MaterialViewDto>> GetMaterialsForStudentAsync(int studentId)
		{
			var allMaterials = await _unitOfWork.Repository<Material>().GetAllAsync();
			var allAssignedCourses = await _unitOfWork.Repository<AssignedCourse>().GetAllAsync();
			var allDoctors = await _unitOfWork.Repository<Doctor>().GetAllAsync(); // هات كل الدكاترة
			var allCourses = await _unitOfWork.Repository<Course>().GetAllAsync(); // هات كل الكورسات

			var studentCourseIds = allAssignedCourses
				.Where(ac => ac.StudentId == studentId)
				.Select(ac => ac.CourseId)
				.ToList();

			var filteredMaterials = allMaterials
				.Where(m => studentCourseIds.Contains(m.CourseId))
				.ToList();

			// بدل ما تـ Map وخلاص، لأ، نجهز الـ Dto بنفسنا:
			var materialDtos = filteredMaterials.Select(m => new MaterialViewDto
			{
				Id = m.Id,
				Title = m.Title,
				UploadedByName = allDoctors.FirstOrDefault(d => d.Id == m.UploadedById)?.Name,
				CourseName = allCourses.FirstOrDefault(c => c.CourseId == m.CourseId)?.Name
			}).ToList();

			return materialDtos;
		}

		//public async Task<IEnumerable<MaterialViewDto>> GetMaterialsForCourseAsync(int studentId, int courseId)
		//{
		//	var allAssignedCourses = await _unitOfWork.Repository<AssignedCourse>().GetAllAsync();

		//	var isAssigned = allAssignedCourses.Any(ac => ac.StudentId == studentId && ac.CourseId == courseId);

		//	if (!isAssigned)
		//	{
		//		throw new Exception("This student is not assigned to the specified course.");
		//	}

		//	var allMaterials = await _unitOfWork.Repository<Material>().GetAllAsync();
		//	var courseMaterials = allMaterials.Where(m => m.CourseId == courseId).ToList();

		//	return _mapper.Map<IEnumerable<MaterialViewDto>>(courseMaterials);
		//}
		public async Task<IEnumerable<MaterialViewDto>> GetMaterialsForCourseAsync(int studentId, int courseId)
		{
			var allAssignedCourses = await _unitOfWork.Repository<AssignedCourse>().GetAllAsync();
			var allMaterials = await _unitOfWork.Repository<Material>().GetAllAsync();
			var allDoctors = await _unitOfWork.Repository<Doctor>().GetAllAsync();
			var allCourses = await _unitOfWork.Repository<Course>().GetAllAsync();

			var isAssigned = allAssignedCourses.Any(ac => ac.StudentId == studentId && ac.CourseId == courseId);

			if (!isAssigned)
			{
				throw new Exception("This student is not assigned to the specified course.");
			}

			var courseMaterials = allMaterials.Where(m => m.CourseId == courseId).ToList();

			var materialDtos = courseMaterials.Select(m => new MaterialViewDto
			{
				Id = m.Id,
				Title = m.Title,
				UploadedByName = allDoctors.FirstOrDefault(d => d.Id == m.UploadedById)?.Name,
				CourseName = allCourses.FirstOrDefault(c => c.CourseId == m.CourseId)?.Name
			}).ToList();

			return materialDtos;
		}

		//public async Task<IEnumerable<MaterialViewDto>> GetMaterialsUploadedByDoctorAsync(int doctorId)
		//{
		//	var allMaterials = await _unitOfWork.Repository<Material>().GetAllAsync(); // بدون شرط

		//	var doctorMaterials = allMaterials
		//		.Where(m => m.UploadedById == doctorId)
		//		.ToList();

		//	var doctorMaterialsDto = doctorMaterials.Select(m => new MaterialViewDto
		//	{
		//		Id = m.Id,
		//		Title = m.Title,
		//		UploadedByName = m.UploadedBy?.Name, // Assuming UploadedBy is navigation property
		//		CourseName = m.Course?.Name // Assuming Course is navigation property
		//	});

		//	return doctorMaterialsDto;
		//}
		public async Task<IEnumerable<MaterialViewDto>> GetMaterialsUploadedByDoctorAsync(int doctorId)
		{
			var doctorMaterials = await _academyContext.Materials
				.Where(m => m.UploadedById == doctorId)
				.Include(m => m.UploadedBy)
				.Include(m => m.Course)
				.ToListAsync();

			var doctorMaterialsDto = doctorMaterials.Select(m => new MaterialViewDto
			{
				Id = m.Id,
				Title = m.Title,
				UploadedByName = m.UploadedBy?.Name,
				CourseName = m.Course?.Name
			});

			return doctorMaterialsDto;
		}





	}
}
