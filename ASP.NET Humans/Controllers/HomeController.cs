using ASP.NET_Humans.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.ML;
using Microsoft.ML.Data;
using RandomNameGen;

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

        
      //  [Authorize]
        public IActionResult GetPeople(string login)
        {
            if (User.Identity.IsAuthenticated)
            {
                var workers = Generators.GenerateWorkersWithPhoto(4, login);
                return View(workers);
            }
            else
            {
                return Redirect("~/Account/Login/");
            }
        }

        public IActionResult Index()
        {
            // Generators.SavePhotosAsync(1);

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

        [HttpPost]
        public IActionResult GetPerson(string name, int age, int salary, Sex sex)
        {
            Worker worker = new Worker();
            worker.Name = name;
            worker.Age = age;
            worker.Salary = salary;
            worker.Sex = sex;

            return View(worker);
        }
    }

}
