using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Models
{
    public class Course
    {
      
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string CourseCode { get; set; }
        public int CreditHours { get; set; }
        public float Credit { get; set; }

        public List<AssignedCourse> Students { get; set; }

        //اي الكورس يضاف بواسطه كورديناتر واحد 
        public Coordinator ManageBy { get; set; }
        public int ManageById { get; set; }
        public List<DoctorCourses> Doctor { get; set; }

    }
}
