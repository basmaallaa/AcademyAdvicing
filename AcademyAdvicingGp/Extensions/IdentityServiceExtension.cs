using Academy.Core.Models.Identity;
using Academy.Repo.Identity;
using Microsoft.AspNetCore.Identity;
using System.Runtime.CompilerServices;

namespace AcademyAdvicingGp.Extensions
{
    public static class IdentityServiceExtension
    {
        public static  IServiceCollection  AddIdentityServices(this IServiceCollection Services)
        {

            Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppIdentityDbContext>();
            Services.AddAuthentication(); //UserManger,SignManger,RoleManger

            return Services;
        }
    }
}
