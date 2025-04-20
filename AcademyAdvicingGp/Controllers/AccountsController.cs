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



        public AccountsController(IFileService fileService, UserManager<AppUser> userManager , ITokenService tokenService,SignInManager<AppUser> signInManager, AcademyContext academyDbContext ,RoleManager<IdentityRole> roleManager)
        {
            _fileService = fileService;
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _academyDbContext = academyDbContext;
            _roleManager = roleManager;
        }
        [HttpPost("RegisterPerson")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> RegisterPerson([FromForm] RegisterDto model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return BadRequest("Email already exists in Identity.");

            bool emailExists = await _academyDbContext.Doctors.AnyAsync(d => d.Email == model.Email) ||
                               await _academyDbContext.Coordinates.AnyAsync(c => c.Email == model.Email) ||
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
                        if (!await _academyDbContext.Coordinates.AnyAsync(c => c.Email == model.Email))
                        {
                            _academyDbContext.Coordinates.Add(new Coordinator
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

            // Update Identity user
            appUser.DisplayName = dto.Name ?? appUser.DisplayName;
            appUser.PhoneNumber = dto.PhoneNumber ?? appUser.PhoneNumber;
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                appUser.Email = dto.Email;
                appUser.UserName = dto.Email;
            }

            var updateResult = await _userManager.UpdateAsync(appUser);
            if (!updateResult.Succeeded)
                return BadRequest(updateResult.Errors.Select(e => e.Description));

            var roles = await _userManager.GetRolesAsync(appUser);
            var role = roles.FirstOrDefault();

            if (role is null)
                return BadRequest("User has no assigned role.");

            // Update corresponding entity in Academy database based on role
            switch (role)
            {
                case "Doctor":
                    var doctor = await _academyDbContext.Doctors.FirstOrDefaultAsync(d => d.Email == email);
                    if (doctor != null)
                    {
                        doctor.Name = dto.Name ?? doctor.Name;
                        doctor.PhoneNumber = dto.PhoneNumber ?? doctor.PhoneNumber;
                        if (!string.IsNullOrWhiteSpace(dto.Email))
                        {
                            doctor.Email = dto.Email;
                            doctor.UserName = dto.Email;
                        }
                        if (newImageName != null)
                        {
                            doctor.ImagePath = newImageName;
                        }
                    }
                    break;

                case "Coordinator":
                    var coordinator = await _academyDbContext.Coordinates.FirstOrDefaultAsync(c => c.Email == email);
                    if (coordinator != null)
                    {
                        coordinator.Name = dto.Name ?? coordinator.Name;
                        coordinator.PhoneNumber = dto.PhoneNumber ?? coordinator.PhoneNumber;
                        if (!string.IsNullOrWhiteSpace(dto.Email))
                        {
                            coordinator.Email = dto.Email;
                            coordinator.UserName = dto.Email;
                        }
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
                        if (!string.IsNullOrWhiteSpace(dto.Email))
                        {
                            studentAffair.Email = dto.Email;
                            studentAffair.UserName = dto.Email;
                        }
                        if (newImageName != null)
                        {
                            studentAffair.ImagePath = newImageName;
                        }
                    }
                    break;

                default:
                    return BadRequest($"Invalid role: {role}");
            }

            await _academyDbContext.SaveChangesAsync();

            var token = await _tokenService.CreateTokenAsync(appUser, _userManager);

            return Ok(new UserDto
            {
                DisplayName = appUser.DisplayName,
                Email = appUser.Email,
                Token = token,
                Roles = roles.ToList(),
                ImagePath = appUser.ImagePath
            });
        }



    }
}

    