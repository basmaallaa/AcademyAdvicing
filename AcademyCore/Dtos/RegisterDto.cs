using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class RegisterDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string DisplayName { get; set; }

    [Required]
    [Phone]
    public string PhoneNumber { get; set; }

    [Required]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%?&])[A-Za-z\d@$!%?&]{8,}$",
        ErrorMessage = "Password must contain at least 1 Uppercase letter, 1 Digit, and 1 Special Character.")]
    public string Password { get; set; }

 
    public IFormFile?ImageFile { get; set; }

    [Required]
    public List<string> Roles { get; set; } // التعديل هنا
}
                