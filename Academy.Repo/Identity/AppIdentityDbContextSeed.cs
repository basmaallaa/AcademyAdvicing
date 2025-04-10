
using Academy.Core.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Academy.Repo.Identity
    {
        public static class AppIdentityDbContextSeed
        {
            public static async Task SeedUserAsync(AppIdentityDbContext context)
            {
                
                if (!context.Roles.Any())
                {
                    var roles = new List<IdentityRole>
                {
                    new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
                    new IdentityRole { Name = "Doctor", NormalizedName = "DOCTOR" },
                    new IdentityRole { Name = "Student", NormalizedName = "STUDENT" },
                    new IdentityRole { Name = "StudentAffair", NormalizedName = "STUDENTAFFAIR" },
                    new IdentityRole { Name = "Coordinator", NormalizedName = "COORDINATOR" }
                };

                    await context.Roles.AddRangeAsync(roles);
                    await context.SaveChangesAsync();
                }

               
                if (!context.Users.Any())
                {
                    var hasher = new PasswordHasher<AppUser>();

                    var user = new AppUser
                    {
                        DisplayName = "Toqa Mahmoud",
                        Email = "toqamahmoud18@gmail.com",
                        UserName = "toqamahmoud.edu",
                        PhoneNumber = "01096976535",
                        NormalizedEmail = "TOQAMAHMOUD18@GMAIL.COM",
                        NormalizedUserName = "TOQAMAHMOUD.EDU",

                    };

                    user.PasswordHash = hasher.HashPassword(user, "PA$$0rd");

                    await context.Users.AddAsync(user);
                    await context.SaveChangesAsync();

                    
                    var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");

                    if (adminRole != null)
                    {
                        var userRole = new IdentityUserRole<string>
                        {
                            UserId = user.Id,
                            RoleId = adminRole.Id
                        };

                        await context.UserRoles.AddAsync(userRole);
                        await context.SaveChangesAsync();
                    }
                }
            }
        }
    }


              