using Microsoft.AspNetCore.Http;

public class UserDto
{
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }

    public string ImagePath { get; set; }
    public List<string> Roles { get; set; } // تعديل هنا بدلاً من string واحد
}
