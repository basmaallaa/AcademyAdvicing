using Academy.Core.Dtos;
using Academy.Core.Enums;
using Academy.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.ServicesInterfaces
{
    public  interface IAvailableCourse
    {
        //Task<AvailableCourseDto> CreateAvailableCourseAsync(AvailableCourseDto availableCourseDto);
        Task CreateAvailableCourseAsync(AvailableCourse availableCourse);

        Task<AvailableCourseDto> UpdateAvailableCourseAsync(int id, AvailableCourseDto updateCAvailableCourseDto);
        Task<ViewAvailableCourseDto> GetAvailableCourseByIdAsync(int id);
        Task<IEnumerable<ViewAvailableCourseDto>> GetAllAvailableCoursesAsync();
        Task<bool> DeleteAvailableCourseAsync(int id);

        Task<bool> IsCourseAvailableAsync(int courseId, int academicYear, Semster semester);
    }
}
