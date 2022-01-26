using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PersonGeneratorApi.Models;

namespace PersonGeneratorApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
 //   [Route("api/")]
    public class WorkerController : ControllerBase
    {

        [HttpGet("rarity/{rarity}")]
        public ActionResult<Worker> Get(Rarity rarity)
        {
            var worker = PersonGenerator.GenerateWorker(rarity);
            return worker;
        }


        [HttpGet("{count}")]
        public IEnumerable<Worker> Get(int count)
        {
            Random rand = new Random();
            List<Worker> workers = new List<Worker>();
            var enumCount = Enum.GetValues(typeof(Rarity)).Length;

            for (int i = 0; i < count; i++)
            {
                Rarity rarity = (Rarity)rand.Next(enumCount);
                workers.Add(PersonGenerator.GenerateWorker(rarity));
            }

            return workers;
        }

    }
}
