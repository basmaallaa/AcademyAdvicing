using Academy.Core.Dtos;
using Academy.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.ServicesInterfaces
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorDtoID>> GetAllDoctorsAsync();
        Task<DoctorDtoID> GetDoctorByIdAsync(int id);
        Task UpdateDoctorAsync(DoctorDtoID doctorDtoId);
        Task<DoctorDto> AddDoctorAsync(DoctorDto doctorDto);

        Task DeleteDoctorAsync(int id);
        Task<IEnumerable<DoctorDtoID>> SearchDoctorsAsync(string? searchTerm);
    }
}
