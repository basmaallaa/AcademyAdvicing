using Academy.Core.Enums;
using Academy.Core.Models.Identity;
using Academy.Core.Service;
using Academy.Core.ServicesInterfaces;
using Academy.Repo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademyAdvicingGp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]

    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AcademyContext _academyDbContext;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;




        public UsersController( UserManager<AppUser> userManager,  SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, AcademyContext academyDbContext)
        {
            
            _userManager = userManager;
            _academyDbContext = academyDbContext;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        [HttpGet("get-all-users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            var userList = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Admin"))
                    continue; // نعدي المستخدم لو Admin

                userList.Add(new UserDto
                {
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return Ok(userList);
        }



        [HttpDelete("delete-user/{email}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound(new { message = "User not found in Identity." });

            var roles = await _userManager.GetRolesAsync(user);

            bool isRemovedFromAny = false;

            foreach (var role in roles.Distinct())
            {
                switch (role)
                {
                    case "Doctor":
                        var doctor = await _academyDbContext.Doctors.FirstOrDefaultAsync(d => d.Email == email);
                        if (doctor != null)
                        {
                            // Delete related Availablecourses
                            var availableCourses = await _academyDbContext.Availablecourses
                                .Where(ac => ac.DoctorId == doctor.Id)
                                .ToListAsync();

                            _academyDbContext.Availablecourses.RemoveRange(availableCourses); // Remove related courses
                            _academyDbContext.Doctors.Remove(doctor);
                            isRemovedFromAny = true;
                        }
                        break;

                    case "Coordinator":
                        var coordinator = await _academyDbContext.Coordinates.FirstOrDefaultAsync(c => c.Email == email);
                        if (coordinator != null)
                        {
                            _academyDbContext.Coordinates.Remove(coordinator);
                            isRemovedFromAny = true;
                        }
                        break;

                    case "StudentAffair":
                        var studentAffair = await _academyDbContext.StudentAffairs.FirstOrDefaultAsync(sa => sa.Email == email);
                        if (studentAffair != null)
                        {
                            _academyDbContext.StudentAffairs.Remove(studentAffair);
                            isRemovedFromAny = true;
                        }
                        break;

                    case "Student":
                        var student = await _academyDbContext.Students.FirstOrDefaultAsync(s => s.Email == email);
                        if (student != null)
                        {
                            // اتأكد من حالته قبل المسح
                            if (student.Status == Status.graduated|| student.Status == Status.Expelled)
                            {
                                _academyDbContext.Students.Remove(student);
                                isRemovedFromAny = true;
                            }
                            else
                            {
                                return BadRequest(new { message = "Cannot delete student unless they are Graduated or Expelled." });
                            }
                        }
                        break;


                    default:
                        break;
                }
            }

            if (isRemovedFromAny)
            {
                await _academyDbContext.SaveChangesAsync();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { message = "Failed to delete user from Identity", errors = result.Errors });

            return Ok(new { message = "User deleted successfully." });
        }

    }
}
