using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Models
{
    public class Material
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Doctor UploadedBy { get; set; }
        public int UploadedById { get; set; }
    }
}
