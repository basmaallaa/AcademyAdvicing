﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Dtos.MaterialsDtos
{
    public class MaterialDto
    {
        //public int Id { get; set; }
        public string Title { get; set; }
        public int UploadedById { get; set; }
        public int CourseId { get; set; }

    }
}
