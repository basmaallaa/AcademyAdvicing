using Academy.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.ServicesInterfaces
{
    public  interface IAvailableCourse
    {
        Task<AvailableCourseDto> CreateAvailableCourseAsync(AvailableCourseDto availableCourseDto);
        Task<AvailableCourseDto> UpdateAvailableCourseAsync(int id, AvailableCourseDto updateCAvailableCourseDto);
        Task<AvailableCourseDto> GetAvailableCourseByIdAsync(int id);
        Task<IEnumerable<AvailableCourseDto>> GetAllAvailableCoursesAsync();
        Task<bool> DeleteAvailableCourseAsync(int id);
    }
}
