using Academy.Core;
using Academy.Core.Dtos;
using Academy.Core.Models;
using Academy.Core.ServicesInterfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Services.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public DoctorService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        public async Task<DoctorDto> AddDoctorAsync(DoctorDto doctorDto)
        {
            // map dto to model
            var doctor = _mapper.Map<Doctor>(doctorDto);

            // add the doctor to the repo 
            await _unitOfWork.Repository<Doctor>().AddAsync(doctor);

            await _unitOfWork.CompleteAsync();

            // map back the model to dto
            return _mapper.Map<DoctorDto>(doctor);
        }

        public async Task DeleteDoctorAsync(int id)
        {
            // retrive the doctor from the DB
            var doctor = await _unitOfWork.Repository<Doctor>().GetAsync(id);

            if (doctor == null)
                throw new KeyNotFoundException($"Doctor with ID {id} not found.");

            _unitOfWork.Repository<Doctor>().Delete(doctor);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<DoctorDtoID>> GetAllDoctorsAsync()
        {
            return _mapper.Map<IEnumerable<DoctorDtoID>>(await _unitOfWork.Repository<Doctor>().GetAllAsync());
        }

        public async Task<DoctorDtoID> GetDoctorByIdAsync(int id)
        {
            var doctor = await _unitOfWork.Repository<Doctor>().GetAsync(id);
            var doctorMapped = _mapper.Map<DoctorDtoID>(doctor);
            return doctorMapped;
        }

        public async Task<IEnumerable<DoctorDtoID>> SearchDoctorsAsync(string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<DoctorDtoID>(); // Return an empty list if the search term is empty or null
            }

            searchTerm = searchTerm.Trim().ToLower(); // Normalize the search term

            // Fetch all doctors from the repository
            var doctors = await _unitOfWork.Repository<Doctor>().GetAllAsync();

            // Filter doctors by name (case-insensitive)
            var filteredDoctors = doctors.Where(d => d.Name.ToLower().Contains(searchTerm)).ToList();

            // Map to DTOs and return
            return _mapper.Map<IEnumerable<DoctorDtoID>>(filteredDoctors);

        }

        public async Task UpdateDoctorAsync(DoctorDtoID doctorDtoid)
        {
            var existingDoctor = await _unitOfWork.Repository<Doctor>().GetAsync(doctorDtoid.Id);

            if (existingDoctor == null)
            {
                throw new KeyNotFoundException($"Doctor with ID {doctorDtoid.Id} not found");
            }
            // Map updated properties from DTO to the existing entity
            _mapper.Map(doctorDtoid, existingDoctor);

            // Update doctor in repository
            _unitOfWork.Repository<Doctor>().Update(existingDoctor);

            await _unitOfWork.CompleteAsync();
        }
    }
}
