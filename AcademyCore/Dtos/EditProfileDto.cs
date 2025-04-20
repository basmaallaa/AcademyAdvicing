using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Dtos
{
    public class EditProfileDto
    {
        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public IFormFile? ImageFile { get; set; }
        public string Email { get; set; }

        public string ArabicFullName { get; set; }

        public string HomeAddress { get; set; }

        public string EmergencyContact { get; set; }

    }
}
