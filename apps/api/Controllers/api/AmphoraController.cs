using Microsoft.AspNetCore.Mvc;
using Amphora.Common.Models;
using Amphora.Api.Contracts;

namespace Amphora.Api.Api.Controllers
{
    [Route("api/amphorae")]
    public class AmphoraController : Controller
    {
        private readonly IEntityStore<Amphora.Common.Models.Amphora> amphoraModelStore;

        public AmphoraController(IEntityStore<Amphora.Common.Models.Amphora> amphoraModelStore)
        {
            this.amphoraModelStore = amphoraModelStore;
        }

        [HttpGet()]
        public IActionResult ListAmphora()
        {
            return Ok(this.amphoraModelStore.List());
        }


        [HttpGet("{id}")]
        public IActionResult GetAmphoraInformation(string id)
        {
            return Ok(this.amphoraModelStore.Get(id));
        }

        [HttpPut()]
        public IActionResult SetAmphora([FromBody] Amphora.Common.Models.Amphora model)
        {
            if (model == null)
            {
                return BadRequest("Invalid Model");
            }
            return Ok(this.amphoraModelStore.Set(model));
        }
    }
}
