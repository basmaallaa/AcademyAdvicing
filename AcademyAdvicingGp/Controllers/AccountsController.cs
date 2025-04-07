using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Academy.Core.Dtos;
using Academy.Core.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace AcademyAdvicingGp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public AccountsController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }




        [HttpPost("Register")]
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


    }

    
}
