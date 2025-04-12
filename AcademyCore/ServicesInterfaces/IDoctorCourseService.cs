using Academy.Core.Dtos;
using Academy.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.ServicesInterfaces
{
    public interface IDoctorCourseService
    {
        Task AddDoctorCourseAsync(AvailableCourse doctorAvailableCourses);
        //Task AddDoctorCoursesAsync(DoctorCourseDto dto);

    }
}
