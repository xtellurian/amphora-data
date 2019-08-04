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


        [HttpPost("{id}/fillJson")]
        public async Task<IActionResult> FillWithJson(string id, [FromBody] IEnumerable<JObject> data)
        {
            await fillerService.FillWithJson(id, data);
            return Ok();
        }

        [HttpPost("{id}/fillBinary")]
        public async Task<IActionResult> FillWithBinary(string id)
        {
            await fillerService.FillWithBinary(id, Request.Body);
            return Ok();
        }
    }
}
