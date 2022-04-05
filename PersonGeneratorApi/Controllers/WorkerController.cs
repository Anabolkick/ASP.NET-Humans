using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OriginalWorker;
using PersonGeneratorApi.Models;

namespace PersonGeneratorApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkerController : ControllerBase
    {
        [HttpGet("rarity/{rarity}")]
        public async Task<ActionResult<GenWorker>> Get(Rarity rarity)
        {
            var worker = await PersonGenerator.GenerateWorkerAsync(rarity);
            return worker;
        }


        [HttpGet("{count}")]
        public async Task<IEnumerable<GenWorker>> Get(int count)
        {
            Random rand = new Random();
            List<GenWorker> workers = new List<GenWorker>();
            var enumCount = Enum.GetValues(typeof(Rarity)).Length;

            for (int i = 0; i < count; i++)
            {
                Rarity rarity = (Rarity)rand.Next(enumCount);
                workers.Add(await PersonGenerator.GenerateWorkerAsync(rarity));
            }

            return workers;
        }

    }
}
