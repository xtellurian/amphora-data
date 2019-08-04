using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Amphora.Api.Api.Controllers
{
    [Route("api/amphorae")]
    public class DrinkAmphoraController : Controller
    {
        private readonly IAmphoraDrinkerService drinkerService;

        public DrinkAmphoraController(IAmphoraDrinkerService drinkerService)
        {
            this.drinkerService = drinkerService;
        }


        [HttpGet("{id}/drinkBinary")]
        public async Task<IActionResult> FillWithBinary(string id)
        {
            var data = await drinkerService.DrinkBinary(id);
            return new FileStreamResult(data, "application/octet-stream");
        }
    }
}
