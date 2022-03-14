using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASP.NET_Humans.Models;
using Microsoft.AspNetCore.Identity;

namespace ASP.NET_Humans.Domain
{
    public static class InitialSetup
    {
        public static async Task InitialSetupAsync(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            await CreateRolesAsync(roleManager, userManager);
            CreateFolders();
        }

        private static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                var role = new IdentityRole("Admin");
                await roleManager.CreateAsync(role);
                await CreateAdminsAsync(userManager);
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                var role = new IdentityRole() { Name = "User" };
                await roleManager.CreateAsync(role);
            }
        }

        private static async Task CreateAdminsAsync(UserManager<User> userManager)
        {
            var admin = new User { UserName = "Admin", Email = "oleg10galysh@gmail.com", EmailConfirmed = true, Id = "admin" };
            var system = new User { UserName = "System", Email = "oleg.10galysh@gmail.com", EmailConfirmed = true, Id = "system" };
            string pass = "Anab@1kick";

            if (userManager.CreateAsync(admin, pass).Result.Succeeded && userManager.CreateAsync(system, pass).Result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
                await userManager.AddToRoleAsync(system, "Admin");
            }
        }

        private static void CreateFolders()
        {
            Directory.CreateDirectory("wwwroot/Images/Users");
        }
    }
}
