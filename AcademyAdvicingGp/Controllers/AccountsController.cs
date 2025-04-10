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

        /*[HttpPost("Register")]
        
        public async Task<ActionResult<UserDto>> Register (RegisterDto model)
        {
            var user = new AppUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split('@')[0],
                PhoneNumber = model.PhoneNumber,
            };
            var Result =await _userManager.CreateAsync(user,model.Password);
            if (!Result.Succeeded) return BadRequest(new ApiResponse(400));

            var ReturnedUser = new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = "ThisWillBeToken"
            };
            
            return Ok(ReturnedUser);
            
        }
        */

        [HttpPost("Register")]
        [Authorize] // التأكد من أن المستخدم مصادق
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto model, [FromQuery] string userType)
        {
            // التحقق من أن المستخدم الحالي لديه الدور المناسب
            var currentUser = await _userManager.GetUserAsync(User);
            var isAuthorized = userType switch
            {
                "Doctor" => await _userManager.IsInRoleAsync(currentUser, "Coordinator"),
                "Student" => await _userManager.IsInRoleAsync(currentUser, "StudentAffair"),
               
            _ => false
            };

            if (!isAuthorized)
                return Forbid(); // المستخدم ليس لديه الصلاحيات المطلوبة

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

            // إضافة الدور المناسب للمستخدم بناءً على userType
            var roleResult = await _userManager.AddToRoleAsync(appUser, userType);
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
    


    /*[HttpPost("RegisterDoctor")]
    [Authorize(Roles ="Coordinator")]
    public async Task<ActionResult<UserDto>> RegisterDoctor(RegisterDto model)
    {
        // التحقق مما إذا كان البريد الإلكتروني مسجلاً بالفعل
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
            return BadRequest("Account is already registered");

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

        // إضافة الدور "Doctor" للمستخدم
        await _userManager.AddToRoleAsync(appUser, "Doctor");

        // إنشاء كائن UserDto لإرجاع معلومات المستخدم
        var returnedUser = new UserDto()
        {
            DisplayName = appUser.DisplayName,
            Email = appUser.Email,
            Token = await _tokenService.CreateTokenAsync(appUser, _userManager)
        };

        return Ok(returnedUser);
    }*/
    /*[Authorize(Roles = "StudentAffair")]
    [HttpPost("RegisterStudent")]
    public async Task<ActionResult<UserDto>> RegisterStudent(RegisterDto model)
    {
        // التحقق مما إذا كان البريد الإلكتروني مسجلاً بالفعل
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
            return BadRequest("Account is already registered");

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

        // إضافة الدور "Doctor" للمستخدم
        await _userManager.AddToRoleAsync(appUser, "Student");

        // إنشاء كائن UserDto لإرجاع معلومات المستخدم
        var returnedUser = new UserDto()
        {
            DisplayName = appUser.DisplayName,
            Email = appUser.Email,
            Token = await _tokenService.CreateTokenAsync(appUser, _userManager)
        };

        return Ok(returnedUser);
    }*/
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

    