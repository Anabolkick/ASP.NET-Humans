using ASP.NET_Humans.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace ASP.NET_Humans.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult GetHuman()
        {
            var workers = Generators.GenerateWorkersWithPhoto(4);
            return View(workers);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            int t = 5;
         //   Generators.SavePhotos(20);
         //   Generators.GetPhotoName("female");
            return View(t);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }



}
