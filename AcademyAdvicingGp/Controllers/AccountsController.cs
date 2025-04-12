using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Academy.Core.Dtos;
using Academy.Core.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using Academy.Core.Service;
using Academy.Services;


namespace AcademyAdvicingGp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
         
       
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountsController(UserManager<AppUser> userManager , ITokenService tokenService,SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        [HttpPost("Register")]
        [Authorize(Roles = "Admin")] // السماح فقط للمسؤول
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto model)
        {
            // التحقق مما إذا كان البريد الإلكتروني مسجلاً بالفعل
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return BadRequest("Email is already registered.");

            // إنشاء كائن AppUser جديد وتعيين الخصائص
            var appUser = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                DisplayName = model.DisplayName,
                PhoneNumber = model.PhoneNumber,
            };

            // إنشاء المستخدم وتعيين كلمة المرور
            var result = await _userManager.CreateAsync(appUser, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(x => x.Description));

            // التحقق من صحة الدور المرسل
            var validRoles = new[] { "Doctor", "Coordinator", "StudentAffair" }; // أضف الأدوار المتاحة هنا
            if (!validRoles.Contains(model.Role))
                return BadRequest("Invalid role.");

            // إضافة الدور للمستخدم
            var roleResult = await _userManager.AddToRoleAsync(appUser, model.Role);
            if (!roleResult.Succeeded)
                return BadRequest(roleResult.Errors.Select(x => x.Description));

            // إنشاء كائن UserDto لإرجاع معلومات المستخدم
            var returnedUser = new UserDto()
            {
                DisplayName = appUser.DisplayName,
                Email = appUser.Email,
                Token = await _tokenService.CreateTokenAsync(appUser, _userManager)
            };

            return Ok(returnedUser);
        }


        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var appUser = await _userManager.FindByEmailAsync(model.Email);
            if (appUser is null) return Unauthorized(new ApiResponse(401));
            var Result = await _signInManager.CheckPasswordSignInAsync(appUser, model.Password, false);
            if (!Result.Succeeded) return Unauthorized(new ApiResponse(401));
            return Ok(new UserDto()
            {
                DisplayName = appUser.DisplayName,
                Email = appUser.Email,
                Token = await _tokenService.CreateTokenAsync(appUser, _userManager)
            });
        }

    }
}

    