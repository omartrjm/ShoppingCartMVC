using Microsoft.AspNetCore.Identity;
using BookUI.Constants;
namespace BookUI.Data
{
    public class DbSeeder
    {
        public static async Task SeedDefaulData(IServiceProvider service)
        {
            var userMgr = service.GetService<UserManager<IdentityUser>>();
            var roleMgr = service.GetService<RoleManager<IdentityRole>>();

            //adding some roles to DataBase.
            await roleMgr.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleMgr.CreateAsync(new IdentityRole(Roles.User.ToString()));

            // create user admin
            var admin = new IdentityUser
            {
                UserName = "admin",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
            };

            // User Exists in datatbase.
            var userInDB = await userMgr.FindByEmailAsync(admin.Email);
            if (userInDB is  null) // if user not exist in database
            {
                await userMgr.CreateAsync(admin, "Admin@123");
                await userMgr.AddToRoleAsync(admin, Roles.Admin.ToString());
                
            }
        }
    }
}
