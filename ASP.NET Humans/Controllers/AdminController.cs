using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASP.NET_Humans.Models;
using ASP.NET_Humans.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Humans.Controllers
{
    public class AdminController : Controller
    {
        private UserManager<User> userManager;
        private RoleManager<IdentityRole> roleManager;

        public AdminController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public IActionResult UsersList()
        {
            var users = userManager.Users.ToList();
            //var assa = userManager.FindByNameAsync("assa").Result;
            //   assa.LockoutEnd = DateTimeOffset.Now + TimeSpan.FromDays(10);
            //   Task.Run(() => userManager.UpdateAsync(assa)).Wait();
            return View(users);

        }

        public IActionResult EditUser(string id)
        {
            var user = userManager.FindByIdAsync(id).Result;
            var roles = userManager.GetRolesAsync(user).Result.ToList();
            EditUserViewModel editUser = new EditUserViewModel(){Email = user.Email, EmailConfirmed = user.EmailConfirmed, Id = user.Id, Login = user.UserName,Roles = roles};
            return View(editUser);
        }

        [HttpPost]
        public IActionResult EditUser(EditUserViewModel model)
        {
            var user = userManager.FindByIdAsync(model.Id).Result;
            user.UserName = model.Login;
            user.Email = model.Email;
            user.Id = model.Id;
            user.Email = model.Email;
            user.EmailConfirmed = model.EmailConfirmed;
           // roleManager.Roles
            var result = userManager.UpdateAsync(user).Result;
            if (result.Succeeded)
            {
               return RedirectToAction("UsersList");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }
        }

    }
}
