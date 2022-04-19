using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Threading.Tasks;
using ASP.NET_Humans.Domain;
using ASP.NET_Humans.Models;
using ASP.NET_Humans.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace ASP.NET_Humans.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<User> userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _DBcontext;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, AppDbContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _DBcontext = context;
            this.userManager = userManager;
        }

        #region Register\EmailConf

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Name };
                // добавляем пользователя
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {

                    var res = userManager.AddToRoleAsync(user, "User").Result;
                    // установка куки
                    await _signInManager.SignInAsync(user, false);

                    string confirmationToken = userManager.GenerateEmailConfirmationTokenAsync(user).Result;

                    string confirmationLink = Url.Action("ConfirmEmail", "Account", new
                    {
                        userid = user.Id,
                        token = confirmationToken
                    },
                        protocol: HttpContext.Request.Scheme);

                    using SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                    client.Credentials = new System.Net.NetworkCredential("peopletwo98@gmail.com", "cs18g1d9");
                    client.EnableSsl = true;

                    MailMessage message = new MailMessage("peopletwo98@gmail.com", user.Email);
                    message.IsBodyHtml = true;
                    message.Body = $"Confirmation link: <a href =\"{confirmationLink}\">Link</a>";
                    message.Subject = "Confirm your email";

                    client.Send(message);


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
                TempData["ActiveToast"] = "SuccessToast('Email confirmed successfully!');";
                return RedirectToAction(controllerName: "Home", actionName: "Index");   //TODO my account
            }
            else
            {
                TempData["ActiveToast"] = $"FailToast('Error while confirming your email!+ {result.Errors}');";
                return RedirectToAction(controllerName: "Home", actionName: "Index");
            }
        }
        #endregion

        #region Login\Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email_Login.ToUpper()) ?? await userManager.FindByEmailAsync(model.Email_Login.ToUpper());
                SignInResult result;
                if (user != null)
                {
                    result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.IsRemember, false);
                }
                else
                {
                    result = SignInResult.Failed;
                    TempData["LoginStatusToast"] = "FailToast('Can`t find account with this login/email!');";
                    return;
                }

                if (result.Succeeded)
                {
                    TempData["LoginStatusToast"] = "SuccessToast('You have successfully logged in.');";
                }
                else
                {
                    TempData["LoginStatusToast"] = "FailToast('Incorrect password!');";
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region GetPeople
        public async Task<IActionResult> GetPeople()         
        {
            var login = User.Identity.Name;

            if (User.Identity is { IsAuthenticated: false })
            {
                return Redirect("~/Home/");
            }
            if (login == null)
            {
                return Redirect("~/Home/");
            }

            User user = userManager.FindByNameAsync(login).Result;
            ViewBag.User = user;
            List<string> workersId = new List<string>();
            List<Worker> workers = _DBcontext.Workers.Where(w => w.UserId == user.Id && w.IsHired == false).ToList();

            #region If workers == 4 --> show them
            if (workers.Count() == 4)
            {
                foreach (var worker in workers)
                {
                    workersId.Add(worker.Id.ToString());
                }
                TempData["Workers"] = workersId;
                return View(workers);
            }
            #endregion

            #region If workers > 4 --> delete while != 4
            if (workers.Count() > 4)
            {
                while (workers.Count > 4)
                {
                    var dbWorker = _DBcontext.Workers.FirstOrDefault(w => w.Id.ToString() == workers[0].UserId);
                    if (dbWorker != null)
                    {
                        dbWorker.UserId = "system";
                    }
                    workers.RemoveAt(0);
                }

                foreach (var worker in workers)
                {
                    workersId.Add(worker.Id.ToString());
                }
                TempData["Workers"] = workersId;
                return View(workers);
            }
            #endregion

            #region If workers < 4 --> generate new
            if (workers.Count < 4)
            {
                workers = GetNewWorkers(user, 4 - workers.Count).Result;
                if (workers == null)
                {
                    return BadRequest($"Error while generate workers. Please, refresh the page and try again");
                }
            }
            #endregion

            foreach (var worker in workers)
            {
                worker.IsHired = false;
                worker.UserId = user.Id;
                _DBcontext.Workers.Add(worker);
                workersId.Add(worker.Id.ToString());
            }

            await _DBcontext.SaveChangesAsync();
            TempData["Workers"] = workersId;

            return View(workers);
        }

        private async Task<List<Worker>> GetNewWorkers(User user, int count) //user to save in user folder
        {
            List<Worker> workers = new List<Worker>();
            try
            {
                await Task.Run(() =>
                {
                    using var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("AccessKey", _configuration["ApiAccessKey"]);
                    HttpResponseMessage response = httpClient.GetAsync($"https://localhost:44320/Worker/{count}").Result;
                    response.EnsureSuccessStatusCode();
                    var result = response.Content.ReadFromJsonAsync(typeof(IEnumerable<Worker>)).Result;
                    workers = (List<Worker>)result;
                });
            }
            catch
            {
                return null;
            }

            //Save image from bytes
            await Task.Run(() =>
            {
                foreach (var worker in workers)
                {
                    var path = $"wwwroot/Images/Users/{user.Id}";
                    Directory.CreateDirectory(path);

                    MemoryStream ms = new MemoryStream(worker.ImageBytes, 0, worker.ImageBytes.Length);
                    ms.Write(worker.ImageBytes, 0, worker.ImageBytes.Length);
                    var image = Image.FromStream(ms, true);
                    image.Save($"{path}/{worker.Id}.jpg");
                }
            });

            return workers;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RefreshGeneratedPeople()
        {
            var login = User.Identity.Name;

            //return old workers to system
            var old_workers_id = TempData["Workers"] as IEnumerable<string>;
            if (old_workers_id == null)
            {
                return BadRequest("Error! Can`t update workers. Please, refresh the page.");
            }
            foreach (var id in old_workers_id)
            {
                var dbWorker = _DBcontext.Workers.FirstOrDefault(w => w.Id.ToString() == id);
                if (dbWorker != null)
                {
                    dbWorker.UserId = "system";
                }
            }
            await _DBcontext.SaveChangesAsync();

            return RedirectToAction("GetPeople");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RefreshSystemPeople()
        {
            //Проверка, есть ли в системе минимум 4 рабочих
            var sysCount = _DBcontext.Workers.Count(w => w.UserId == "system");
            if (sysCount < 4)
            {
                return BadRequest("Error! Count of system workers less then 4.");
            }

            //Сохраняет айди старых рабочих, которые потом будут перенесены в систему
            var old_workers_id = TempData["Workers"] as IEnumerable<string>;
            if (old_workers_id == null)
            {
                return BadRequest("Error! Can`t update workers. Please, refresh the page.");
            }

            //Достает случайных рабочих с системы
            List<Worker> allWorkersList = _DBcontext.Workers.Where(w => w.UserId == "system").ToList();
            List<string> workers_id = new List<string>();

            Random rand = new Random();
            for (int i = 0; i < 4; i++)
            {
                var rand_num = rand.Next(0, allWorkersList.Count - 1);
                workers_id.Add(allWorkersList[rand_num].Id.ToString());
                allWorkersList.RemoveAt(rand_num);
            }

            var UserId = userManager.Users.FirstOrDefault(u => u.UserName == User.Identity.Name)?.Id;
            if (UserId == null)
            {
                return BadRequest("Error! Re-login to your account, please!");
            }

            foreach (var id in workers_id)
            {
                var dbWorker = _DBcontext.Workers.FirstOrDefault(w => w.Id.ToString() == id);
                if (dbWorker != null)
                {
                    dbWorker.UserId = UserId;
                    dbWorker.IsHired = false;
                }
            }

            //Переносит старых рабочих в систему
            foreach (var id in old_workers_id)
            {
                var dbWorker = _DBcontext.Workers.FirstOrDefault(w => w.Id.ToString() == id);
                if (dbWorker != null)
                {
                    dbWorker.UserId = "system";
                }
            }


            await _DBcontext.SaveChangesAsync();
            return RedirectToAction("GetPeople");
        }
        #endregion
    }
}
