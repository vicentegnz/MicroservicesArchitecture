using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microservice1.Models;

namespace MicroserviceExample1Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class Microservice1Controller : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var items = new List<Item>()
            {
                new Item { Id = 1, Name = "Cog" },
                new Item { Id = 2, Name = "Gear"},
                new Item { Id = 3, Name = "Sprocket" }
            };

            return Ok(items);
        }
    }
}
