using Academy.Core.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Repo.Identity
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManger)
        {
            if(!userManger.Users.Any())
            {
                var User = new AppUser()
                {
                    DisplayName = "toqa Mahmoud",
                    Email = "toqamahmoud18@gmail.com",
                    UserName = "toqamahmoud.edu",
                    PhoneNumber = "01096976535"
                };

                await userManger.CreateAsync(User , "PA$$0rd");
            }
        }
    }
}
