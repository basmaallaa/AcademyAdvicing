using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Dtos.MaterialsDtos
{
    public class MaterialViewDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string UploadedByName { get; set; }
        public string CourseName { get; set; }
    }
}
