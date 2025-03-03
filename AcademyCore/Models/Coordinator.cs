using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Models
{
    public class Coordinator : Person
    {
        //اي الكورديناتور لي انه يضيف اكتر من كورس 
        public List<Course> Courses { get; set; }
        public List<Doctor> Doctors { get; set; }
        public List<Report> Reports { get; set; }
        public List<ScheduleTimeTable> ScheduleTimeTables { get; set; }
        public List<FinalExamTimeTable> finalExamTimeTables { get; set; }

    }
}
