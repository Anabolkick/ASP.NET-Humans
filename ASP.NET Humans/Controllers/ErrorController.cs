using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Humans.Controllers
{
    public class ErrorController  : Controller
    {
        public IActionResult Error_404()
        {
            return View();
        }
    }
}
