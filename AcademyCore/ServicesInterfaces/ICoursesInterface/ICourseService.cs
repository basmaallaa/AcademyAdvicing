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
        
    }
}
