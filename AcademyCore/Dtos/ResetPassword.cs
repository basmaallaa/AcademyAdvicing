using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Dtos
{
    public class ResetPassword
    {
        [Required]
        public string NewPassword { get; set; } = null!;

        [Compare("NewPassword", ErrorMessage = "The Password and Configration password do not match.")]
        public string ConfirmPassword { get; set; } = null!;
        public string Email { get; set; } = null!;
        
    }
}
