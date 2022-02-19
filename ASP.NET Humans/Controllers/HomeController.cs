using System;
using System.Buffers.Text;
using System.Collections.Generic;
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

        public HomeController(ILogger<HomeController> logger, IOptions<AnabolkickCompany> AnabComp, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _logger = logger;
            Name = AnabComp.Value.Name;
            Developer = AnabComp.Value.Developer;
            this.roleManager = roleManager;
            this.userManager = userManager;

        }

        // [Authorize]
        public async Task<IActionResult> GetPeople(string login)
        {
            if (User.Identity.IsAuthenticated)
            {
                List<Worker> workersList = new List<Worker>();
                User user = userManager.FindByNameAsync(login).Result;
                ViewBag.User = user;


                IEnumerable<Worker> workers;
                await Task.Run(() =>
                {
                    using var httpClient = new HttpClient();
                    HttpResponseMessage response = httpClient.GetAsync("https://localhost:44320/Worker/4").Result;
                    response.EnsureSuccessStatusCode();
                    var result = response.Content.ReadFromJsonAsync(typeof(IEnumerable<Worker>)).Result;
                    workers = (IEnumerable<Worker>)result;

                    workersList = workers.ToList();
                });

                await Task.Run(() =>
                {
                    foreach (var worker in workersList)
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

                return View(workersList);
            }
            else
            {
                return Redirect("~/Account/Login/");
            }
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
