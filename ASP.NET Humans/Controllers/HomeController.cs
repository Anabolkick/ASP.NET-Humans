using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ASP.NET_Humans.Domain;
using ASP.NET_Humans.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ASP.NET_Humans.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public string Developer { get; }
        public string Name { get; }


        private RoleManager<IdentityRole> roleManager;
        private UserManager<User> userManager;
        private AppDbContext DBcontext;

        public HomeController(ILogger<HomeController> logger, IOptions<AnabolkickCompany> AnabComp, RoleManager<IdentityRole> roleManager, UserManager<User> userManager, AppDbContext context)
        {
            _logger = logger;
            Name = AnabComp.Value.Name;
            Developer = AnabComp.Value.Developer;
            this.roleManager = roleManager;
            this.userManager = userManager;
            DBcontext = context;

        }

        public async Task<IActionResult> GetPeople(string login)
        {

            if (User.Identity is { IsAuthenticated: false })
            {
                return Redirect("~/Account/Login/");
            }

            if (login == null)   //todo проверку при проходе запроса
            {
                return Redirect("~/Home/");
            }

            User user = userManager.FindByNameAsync(login).Result;
            ViewBag.User = user;

            List<Worker> workers;
            List<string> workers_id = new List<string>();

            workers = DBcontext.Workers.Where(w => w.UserId == user.Id && w.IsHired == false).ToList();

            if (workers.Count() == 4)
            {
                foreach (var worker in workers)
                {
                    workers_id.Add(worker.Id.ToString());
                }
                TempData["Workers"] = workers_id;
                return View(workers);
            }

            // если больше 4, то отдавать системе, пока не будет 4
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
                    workers_id.Add(worker.Id.ToString());
                }

                TempData["Workers"] = workers_id;
                return View(workers);
            }

            // если их нет, то сгенерировать новых
            workers = new List<Worker>();
            workers_id = new List<string>();

            await Task.Run(() =>
            {
                using var httpClient = new HttpClient();
                HttpResponseMessage response = httpClient.GetAsync("https://localhost:44320/Worker/4").Result;
                response.EnsureSuccessStatusCode();
                var result = response.Content.ReadFromJsonAsync(typeof(IEnumerable<Worker>)).Result;
                workers = (List<Worker>)result;
            });
            await Task.Run(() =>
            {
                foreach (var worker in workers)
                {
                    //Save Image                         
                    var path = $"wwwroot/Images/Users/{user.Id}";
                    Directory.CreateDirectory(path);

                    MemoryStream ms = new MemoryStream(worker.ImageBytes, 0, worker.ImageBytes.Length);
                    ms.Write(worker.ImageBytes, 0, worker.ImageBytes.Length);
                    var image = Image.FromStream(ms, true);
                    image.Save($"{path}/{worker.Id}.jpg");
                }
            });

            foreach (var worker in workers)
            {
                worker.IsHired = false;
                worker.UserId = user.Id;
                DBcontext.Workers.Add(worker);
                workers_id.Add(worker.Id.ToString());
            }

            await DBcontext.SaveChangesAsync();
            TempData["Workers"] = workers_id;

            return View(workers);
        }


        [HttpPost]
        public async Task<IActionResult> RefreshPeople(string login)
        {
            var workers_id = TempData["Workers"] as IEnumerable<string>;
            if (workers_id == null)
            {
                return BadRequest("Error!!!RefreshPeople Workers");
            }
            foreach (var id in workers_id)
            {
                var dbWorker = DBcontext.Workers.FirstOrDefault(w => w.Id.ToString() == id);
                if (dbWorker != null)
                {
                    dbWorker.UserId = "system";
                }
            }
            await DBcontext.SaveChangesAsync();
            return RedirectToAction("GetPeople", new { login = login });
        }

        public IActionResult Index()
        {
            var temp = (Name, Developer);
            return View(temp);
        }

        public string Tutor()
        {
            var elem = Request.Headers;
            string res = "";
            foreach (var VARIABLE in elem)
            {
                res += $"{VARIABLE.Key}  - {VARIABLE.Value} \n";
            }

            res += "\n request \n";
            return res;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // [HttpPost]
        //public IActionResult GetPerson(string name, int age, int salary, Sex sex)
        //{
        //    Worker worker = new Worker();
        //    worker.Name = name;
        //    worker.Age = age;
        //    worker.Salary = salary;
        //    worker.Sex = sex;

        //    return View(worker);
        //}
    }
}
