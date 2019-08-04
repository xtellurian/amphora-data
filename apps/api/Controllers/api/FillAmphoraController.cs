using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Amphora.Api.Api.Controllers
{
    [Route("api/amphorae")]
    public class FillAmphoraController : Controller
    {
        private readonly IAmphoraFillerService fillerService;

        public FillAmphoraController(IAmphoraFillerService fillerService)
        {
            this.fillerService = fillerService;
        }


        [HttpPost("{id}/fill")]
        public async Task<IActionResult> GetAmphoraInformation(string id, [FromBody] IEnumerable<JObject> data)
        {
            await fillerService.FillWithJson(id, data);
            return Ok();
        }
    }
}
