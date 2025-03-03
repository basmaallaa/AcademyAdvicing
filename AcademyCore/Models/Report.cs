using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Models
{
    public class Report
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string FilePath { get; set; }
        public Coordinator GenerateBy { get; set; }
        public int GenerateById { get; set; }
    }
}
