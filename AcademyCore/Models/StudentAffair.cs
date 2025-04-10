using Academy.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Models
{
    public class StudentAffair : Person
    {
        public List<Student> Students { get; set; }
    }
}
