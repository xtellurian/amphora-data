using Microsoft.AspNetCore.Mvc;
using Amphora.Common.Models;
using Amphora.Api.Contracts;

namespace Amphora.Api.Api.Controllers
{
    [Route("api/amphorae")]
    public class AmphoraController : Controller
    {
        private readonly IAmphoraEntityStore<AmphoraModel> amphoraModelStore;

        public AmphoraController(IAmphoraEntityStore<AmphoraModel> amphoraModelStore)
        {
            this.amphoraModelStore = amphoraModelStore;
        }

        [HttpGet()]
        public IActionResult ListAmphoraIds()
        {
            return Ok(this.amphoraModelStore.ListIds());
        }


        [HttpGet("{id}")]
        public IActionResult GetAmphoraInformation(string id)
        {
            return Ok(this.amphoraModelStore.Get(id));
        }

        [HttpPut()]
        public IActionResult SetAmphora([FromBody] AmphoraModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid Model");
            }
            return Ok(this.amphoraModelStore.Set(model));
        }
    }
}
