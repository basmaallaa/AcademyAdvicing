using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Academy.Core.Dtos;
using Academy.Core.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using Academy.Core.Service;
using Academy.Services;
using Academy.Core.Models;
using Academy.Repo.Data;
using Microsoft.EntityFrameworkCore;
using Academy.Core.ServicesInterfaces;
using Academy.Services.Services;
using System.Security.Claims;
using Azure;
using Academy.Core.Models.Email;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;



namespace AcademyAdvicingGp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
         
       
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AcademyContext _academyDbContext;
        private readonly IFileService _fileService;
        private readonly IEmailService _emailService;



        public AccountsController(IFileService fileService, UserManager<AppUser> userManager , ITokenService tokenService,
            SignInManager<AppUser> signInManager, AcademyContext academyDbContext ,RoleManager<IdentityRole> roleManager,
            IEmailService emailService)
        {
            _fileService = fileService;
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _academyDbContext = academyDbContext;
            _roleManager = roleManager;
            _emailService = emailService; 
        }
        [HttpPost("RegisterPerson")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> RegisterPerson([FromForm] RegisterDto model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return BadRequest("Email already exists in Identity.");

            bool emailExists = await _academyDbContext.Doctors.AnyAsync(d => d.Email == model.Email) ||
                               await _academyDbContext.Coordinator.AnyAsync(c => c.Email == model.Email) ||
                               await _academyDbContext.StudentAffairs.AnyAsync(sa => sa.Email == model.Email);

            if (emailExists)
                return BadRequest("Email already exists in Academy database.");

            // هنا هنتعامل مع الصورة عادي سواء null أو موجودة
            string? imageFileName = null;
            if (model.ImageFile is not null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                if (model.ImageFile.Length > 1 * 1024 * 1024)
                    return BadRequest("Image size should not exceed 1 MB.");

                try
                {
                    imageFileName = await _fileService.SaveFileAsync(model.ImageFile, allowedExtensions);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Image upload failed: {ex.Message}");
                }
            }

            var appUser = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                DisplayName = model.DisplayName,
                PhoneNumber = model.PhoneNumber,
                ImagePath = imageFileName // null أو اسم الصورة
            };

            var result = await _userManager.CreateAsync(appUser, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            foreach (var role in model.Roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    return BadRequest($"Role '{role}' does not exist.");

                var addToRoleResult = await _userManager.AddToRoleAsync(appUser, role);
                if (!addToRoleResult.Succeeded)
                    return BadRequest(addToRoleResult.Errors.Select(e => e.Description));

                switch (role)
                {
                    case "Doctor":
                        if (!await _academyDbContext.Doctors.AnyAsync(d => d.Email == model.Email))
                        {
                            _academyDbContext.Doctors.Add(new Doctor
                            {
                                Name = model.DisplayName,
                                UserName = model.Email,
                                Email = model.Email,
                                PhoneNumber = model.PhoneNumber,
                                ImagePath = imageFileName
                            });
                        }
                        break;

                    case "Coordinator":
                        if (!await _academyDbContext.Coordinator.AnyAsync(c => c.Email == model.Email))
                        {
                            _academyDbContext.Coordinator.Add(new Coordinator
                            {
                                Name = model.DisplayName,
                                UserName = model.Email,
                                Email = model.Email,
                                PhoneNumber = model.PhoneNumber,
                                ImagePath = imageFileName
                            });
                        }
                        break;

                    case "StudentAffair":
                        if (!await _academyDbContext.StudentAffairs.AnyAsync(sa => sa.Email == model.Email))
                        {
                            _academyDbContext.StudentAffairs.Add(new StudentAffair
                            {
                                Name = model.DisplayName,
                                UserName = model.Email,
                                Email = model.Email,
                                PhoneNumber = model.PhoneNumber,
                                ImagePath = imageFileName
                            });
                        }
                        break;

                    default:
                        return BadRequest($"Invalid role: {role}");
                }
            }

            await _academyDbContext.SaveChangesAsync();

            var token = await _tokenService.CreateTokenAsync(appUser, _userManager);

            return Ok(new UserDto
            {
                DisplayName = appUser.DisplayName,
                Email = appUser.Email,
                Token = token,
                Roles = model.Roles
            });
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var appUser = await _userManager.FindByEmailAsync(model.Email);
            if (appUser is null) return Unauthorized(new ApiResponse(401));

            var result = await _signInManager.CheckPasswordSignInAsync(appUser, model.Password, false);
            if (!result.Succeeded) return Unauthorized(new ApiResponse(401));

            // جلب الدور
            var roles = await _userManager.GetRolesAsync(appUser);
            //var role = roles.FirstOrDefault();
      
            return Ok(new UserDto()
            {
                DisplayName = appUser.DisplayName,
                Email = appUser.Email,
                Token = await _tokenService.CreateTokenAsync(appUser, _userManager),
                Roles = roles.ToList()
            });
        }


        [HttpPut("EditProfile")]
        [Authorize]
        public async Task<ActionResult<UserDto>> EditProfile([FromForm] EditProfileDto dto)
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
                return BadRequest("Email not found in token.");

            var appUser = await _userManager.FindByEmailAsync(email);
            if (appUser == null)
                return NotFound("User not found");

            string? newImageName = null;

            if (dto.ImageFile is not null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

                if (dto.ImageFile.Length > 1 * 1024 * 1024)
                    return BadRequest("Image size should not exceed 1 MB.");

                try
                {
                    newImageName = await _fileService.SaveFileAsync(dto.ImageFile, allowedExtensions);
                    appUser.ImagePath = newImageName;
                }
                catch (Exception ex)
                {
                    return BadRequest($"Image upload failed: {ex.Message}");
                }
            }

            // Update Identity user (بدون تعديل الإيميل)
            appUser.DisplayName = dto.Name ?? appUser.DisplayName;
            appUser.PhoneNumber = dto.PhoneNumber ?? appUser.PhoneNumber;

            var updateResult = await _userManager.UpdateAsync(appUser);
            if (!updateResult.Succeeded)
                return BadRequest(updateResult.Errors.Select(e => e.Description));

            var roles = await _userManager.GetRolesAsync(appUser);
            if (roles == null || !roles.Any())
                return BadRequest("User has no assigned roles.");

            foreach (var role in roles)
            {
                switch (role)
                {
                    case "Doctor":
                        var doctor = await _academyDbContext.Doctors.FirstOrDefaultAsync(d => d.Email == email);
                        if (doctor != null)
                        {
                            doctor.Name = dto.Name ?? doctor.Name;
                            doctor.PhoneNumber = dto.PhoneNumber ?? doctor.PhoneNumber;
                            doctor.ArabicFullName = dto.ArabicFullName ?? doctor.ArabicFullName;
                            doctor.EmergencyContact = dto.EmergencyContact ?? doctor.EmergencyContact;
                            doctor.HomeAddress = dto.HomeAddress ?? doctor.HomeAddress;

                            if (newImageName != null)
                            {
                                doctor.ImagePath = newImageName;
                            }
                        }
                        break;

                    case "Coordinator":
                        var coordinator = await _academyDbContext.Coordinator.FirstOrDefaultAsync(c => c.Email == email);
                        if (coordinator != null)
                        {
                            coordinator.Name = dto.Name ?? coordinator.Name;
                            coordinator.PhoneNumber = dto.PhoneNumber ?? coordinator.PhoneNumber;
                            coordinator.ArabicFullName = dto.ArabicFullName ?? coordinator.ArabicFullName;
                            coordinator.EmergencyContact = dto.EmergencyContact ?? coordinator.EmergencyContact;
                            coordinator.HomeAddress = dto.HomeAddress ?? coordinator.HomeAddress;

                            if (newImageName != null)
                            {
                                coordinator.ImagePath = newImageName;
                            }
                        }
                        break;

                    case "StudentAffair":
                        var studentAffair = await _academyDbContext.StudentAffairs.FirstOrDefaultAsync(sa => sa.Email == email);
                        if (studentAffair != null)
                        {
                            studentAffair.Name = dto.Name ?? studentAffair.Name;
                            studentAffair.PhoneNumber = dto.PhoneNumber ?? studentAffair.PhoneNumber;
                            studentAffair.ArabicFullName = dto.ArabicFullName ?? studentAffair.ArabicFullName;
                            studentAffair.EmergencyContact = dto.EmergencyContact ?? studentAffair.EmergencyContact;
                            studentAffair.HomeAddress = dto.HomeAddress ?? studentAffair.HomeAddress;

                            if (newImageName != null)
                            {
                                studentAffair.ImagePath = newImageName;
                            }
                        }
                        break;


                    case "Student":
                        var student = await _academyDbContext.Students.FirstOrDefaultAsync(sa => sa.Email == email);
                        if (student != null)
                        {
                            student.Name = dto.Name ?? student.Name;
                            student.PhoneNumber = dto.PhoneNumber ?? student.PhoneNumber;
                            student.ArabicFullName = dto.ArabicFullName ?? student.ArabicFullName;
                            student.EmergencyContact = dto.EmergencyContact ?? student.EmergencyContact;
                            student.HomeAddress = dto.HomeAddress ?? student.HomeAddress;

                            if (newImageName != null)
                            {
                                student.ImagePath = newImageName;
                            }
                        }
                        break;

                    default:
                        // Handle other roles if needed
                        break;
                }
            }

            await _academyDbContext.SaveChangesAsync();

            var token = await _tokenService.CreateTokenAsync(appUser, _userManager);

            return Ok(new UserDto
            {
                DisplayName = appUser.DisplayName,
                Email = appUser.Email, // لعرض الإيميل فقط، بدون تغييره
                Token = token,
                Roles = roles.ToList(),
                ImagePath = appUser.ImagePath
            });
        }

        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email not found in token.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound("User not found.");

            var passwordCheck = await _userManager.CheckPasswordAsync(user, dto.OldPassword);
            if (!passwordCheck)
                return BadRequest("Old password is incorrect.");

            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return Ok("Password changed successfully.");
        }




        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([Required] String email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = Encoding.UTF8.GetBytes(token);
                var validToken = WebEncoders.Base64UrlEncode(encodedToken);

                var forgotPasswordlink = Url.Action(nameof(ResetPassword), "Accounts" , new {token, email=user.Email},Request.Scheme );

                if (string.IsNullOrEmpty(forgotPasswordlink))
                {
                    return BadRequest(new { message = "Failed to generate password reset link." });
                }

                var message = new Message(
           new List<string> { user.Email! },
           "Forgot Password link",
           forgotPasswordlink!
           );
                await _emailService.SendEmailAsync(message);
                return Ok(new { message = $"Password Changed Request is sent on Email {user.Email}.Please open you email & clik on the link " });
            }
            return BadRequest(new { message = $"Failed to send Password Reset Request to Email. Please try again later." });

        }

        [HttpGet("resetPassword")]

        public async Task<IActionResult> ResetPassword (string token , string email)
        {
            var model = new ResetPassword { Token = token, Email = email };
            return Ok(new { model });  
        }

        [HttpPost("resetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user != null)
            {
                var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
                if (!resetPassResult.Succeeded)
                {
                    foreach(var error in resetPassResult.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return Ok(ModelState);
                }

                return Ok(new { message = $"Password has been Changed " });
            }
            return BadRequest(new { message = $"Failed to send Password Reset Request to Email. Please try again later." });

        }

        [HttpGet("test email")]
        
        public async Task<IActionResult> TestEmail()
        {
            var message = new Message(
            new List<string> { "basmaalaa157@gmail.com" }, 
            "Test Email",                                                      
            "<h1>This is a test email</h1>"                                   
            );
            await _emailService.SendEmailAsync(message);
            return Ok(new { message = "Email Sent Successfully" });
        } 

    }
    
}

    