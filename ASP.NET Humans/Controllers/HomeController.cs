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
using Microsoft.Extensions.Options;
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


        public HomeController(ILogger<HomeController> logger, IOptions<AnabolkickCompany> AnabComp)
        {
            _logger = logger;
            Name = AnabComp.Value.Name;
            Developer = AnabComp.Value.Developer;
        }

      //  [HttpPost]
        [Route("Home/GetHuman/{id}")]
        public IActionResult GetHuman(string id)
        {
            var workers = Generators.GenerateWorkersWithPhoto(4, id);
            return View(workers);
        }

        public IActionResult Index()
        {
            // Generators.SavePhotosAsync(1);

            var temp = (Name, Developer);
            return View(temp);
        }

        public IActionResult Tutor()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [Route("Home/GetPerson/{name}/{age}/{sex}")]
        public IActionResult GetPerson(string name, int age, Sex sex)
        {
            Worker worker = new Worker();
            worker.Name = name;
            worker.Age = age;
            worker.Sex = sex;
            return View(worker);
        }

        //public IActionResult TestThis(Worker wk)
        //{
        //    var worker = Generators.GenerateWorker();
        //    //worker.Age = wk.Age;
        //    worker.Name = wk.Name;
        //    Response.Redirect($"/Home/GetPerson&worker={wk.Name}");

        //    var s = Request.QueryString.Value;
        //    return View($"GetPerson&worker={wk.Name}", worker);
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error_404()
        {
            return View();
        }
    }

}
