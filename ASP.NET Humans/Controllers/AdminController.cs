using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASP.NET_Humans.Models;
using ASP.NET_Humans.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Humans.Controllers
{
    [Authorize(Roles = "Admin")]
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
            var allRoles = roleManager.Roles.ToList();
            var userRoles = userManager.GetRolesAsync(user).Result.ToList();
            EditUserViewModel editUser = new EditUserViewModel() { Email = user.Email, EmailConfirmed = user.EmailConfirmed, Id = user.Id, Login = user.UserName, Roles = userRoles, AllRoles = allRoles };
            return View(editUser);
        }

        [HttpPost]
        public IActionResult EditUser(EditUserViewModel model, List<string> picked_roles)
        {
            var user = userManager.FindByIdAsync(model.Id).Result;
            user.UserName = model.Login;
            user.Email = model.Email;
            user.Id = model.Id;
            user.Email = model.Email;
            user.EmailConfirmed = model.EmailConfirmed;

            var userRoles = userManager.GetRolesAsync(user).Result;
            var addedRoles = picked_roles.Except(userRoles);
            var removedRoles = userRoles.Except(picked_roles);

            var result = userManager.UpdateAsync(user).Result;

            Task.Run(() => userManager.AddToRolesAsync(user, addedRoles)).Wait();
            Task.Run(() => userManager.RemoveFromRolesAsync(user, removedRoles)).Wait();

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

        public IActionResult DeleteUser(string id)
        {
            var user = userManager.FindByIdAsync(id).Result;
            var result = userManager.DeleteAsync(user).Result;

            if (result.Succeeded)
            {
                return RedirectToAction("UsersList");
            }
            else
            {
                return Problem(result.Errors.ToString());
            }
        }

    }
}
