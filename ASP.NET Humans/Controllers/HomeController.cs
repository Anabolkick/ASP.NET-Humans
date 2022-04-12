using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ASP.NET_Humans.Domain;
using ASP.NET_Humans.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ASP.NET_Humans.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private RoleManager<IdentityRole> roleManager;
        private UserManager<User> userManager;
        private AppDbContext DBcontext;

        public HomeController(IConfiguration configuration, RoleManager<IdentityRole> roleManager, UserManager<User> userManager, AppDbContext context)
        {
            _configuration = configuration;
            this.roleManager = roleManager;
            this.userManager = userManager;
            DBcontext = context;
        }

        public async Task<IActionResult> GetPeople()         //TODO Other class
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
            List<Worker> workers = DBcontext.Workers.Where(w => w.UserId == user.Id && w.IsHired == false).ToList();

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
                    var dbWorker = DBcontext.Workers.FirstOrDefault(w => w.Id.ToString() == workers[0].UserId);
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
                workers = GenerateNewWorkers(user, 4 - workers.Count).Result;
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
                DBcontext.Workers.Add(worker);
                workersId.Add(worker.Id.ToString());
            }

            await DBcontext.SaveChangesAsync();
            TempData["Workers"] = workersId;

            return View(workers);
        }

        private async Task<List<Worker>> GenerateNewWorkers(User user, int count)
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
        public async Task<IActionResult> RefreshNewPeople()
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
                var dbWorker = DBcontext.Workers.FirstOrDefault(w => w.Id.ToString() == id);
                if (dbWorker != null)
                {
                    dbWorker.UserId = "system";
                }
            }
            await DBcontext.SaveChangesAsync();

            return RedirectToAction("GetPeople");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RefreshSystemPeople()
        {
            //Проверка, есть ли в системе минимум 4 рабочих
            var sysCount = DBcontext.Workers.Count(w => w.UserId == "system");
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
            List<Worker> allWorkersList = DBcontext.Workers.Where(w => w.UserId == "system").ToList();
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
                var dbWorker = DBcontext.Workers.FirstOrDefault(w => w.Id.ToString() == id);
                if (dbWorker != null)
                {
                    dbWorker.UserId = UserId;
                    dbWorker.IsHired = false;
                }
            }

            //Переносит старых рабочих в систему
            foreach (var id in old_workers_id)
            {
                var dbWorker = DBcontext.Workers.FirstOrDefault(w => w.Id.ToString() == id);
                if (dbWorker != null)
                {
                    dbWorker.UserId = "system";
                }
            }


            await DBcontext.SaveChangesAsync();
            return RedirectToAction("GetPeople");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
     
    }

}
