using Academy.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.ServicesInterfaces.ICoursesInterface
{
    public interface ICourseService
    {
   
        Task<CreateCourseDto> CreateCourseAsync(CreateCourseDto createCourseDto);
        Task<CreateCourseDto> UpdateCourseAsync(int id, CreateCourseDto updateCourseDto);
        Task<CreateCourseDto> GetCourseByIdAsync(int id);
        Task<IEnumerable<CreateCourseDto>> GetAllCoursesAsync();
        //Task<bool> DeleteCourseAsync(int id);

    }
}
