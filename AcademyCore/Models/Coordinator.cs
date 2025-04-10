using Academy.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Models
{
    public class Coordinator : Person
    {
     
 
        public List<Course> Courses { get; set; }
        public List<Doctor> Doctors { get; set; }
        public List<Report> Reports { get; set; }
        public List<ScheduleTimeTable> ScheduleTimeTables { get; set; }
        public List<FinalExamTimeTable> finalExamTimeTables { get; set; }

       
    }
}
