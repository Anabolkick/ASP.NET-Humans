using ASP.NET_Humans.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.ML;
using Microsoft.ML.Data;

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

        public IActionResult GetHuman()
        {
            var workers = Generators.GenerateWorkersWithPhoto(4);
            return View(workers);
        }

        public IActionResult Index()
        {
            var temp = (Name, Developer);
            return View(temp);
        }

        public IActionResult Privacy()
        {
            int t = 5;
            ViewData["lala"] = "lalalala";
            var worker = Generators.GenerateWorker();
            ViewData["worker"] = worker;
            //   Generators.SavePhotos(20);
            //   Generators.GetPhotoName("female");
            return View(t);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error_404()
        {
            return View();
        }
    }



}
