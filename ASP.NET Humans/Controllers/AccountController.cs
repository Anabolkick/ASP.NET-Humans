﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using ASP.NET_Humans.Models;
using ASP.NET_Humans.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace ASP.NET_Humans.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Name};
                // добавляем пользователя
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {

                    var res = userManager.AddToRoleAsync(user, "User").Result;
                    // установка куки
                    await signInManager.SignInAsync(user, false);

                    string confirmationToken = userManager.GenerateEmailConfirmationTokenAsync(user).Result;

                    string confirmationLink = Url.Action("ConfirmEmail", "Account", new
                        {
                            userid = user.Id,
                            token = confirmationToken
                        },
                        protocol: HttpContext.Request.Scheme);

                    SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                    client.Credentials = new System.Net.NetworkCredential("oleg10galysh@gmail.com", "kkronbmduhpvrach");
                    client.EnableSsl = true;
                    client.Send("oleg10galysh@gmail.com", user.Email, "Confirm your email", confirmationLink);


                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        public IActionResult ConfirmEmail(string userid, string token)
        {
            User user = userManager.FindByIdAsync(userid).Result;
            var result = userManager.ConfirmEmailAsync(user, token).Result;

            if (result.Succeeded)
            {
                return Problem("Email confirmed successfully!");
            }
            else
            {
                return Problem("Error while confirming your email!" + "\n" + result.Errors);
            }
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email_Login.ToUpper()) ?? await userManager.FindByEmailAsync(model.Email_Login.ToUpper());
                SignInResult result;
                if (user != null)
                { 
                    result = await signInManager.PasswordSignInAsync(user.UserName, model.Password, model.IsRemember, false);
                }
                else
                {
                    result = SignInResult.Failed;
                    ModelState.AddModelError("", "Can`t find account with this login/email!");
                    return View();
                }


                if (result.Succeeded)
                {
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect password!");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
