using Microsoft.AspNetCore.Mvc;
using Amphora.Common.Models;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using System.Threading.Tasks;

namespace Amphora.Api.Api.Controllers
{
    [Route("api/amphorae")]
    public class AmphoraController : Controller
    {
        private readonly IEntityStore<Amphora.Common.Models.Amphora> amphoraEntityStore;
        private readonly IDataStore<Common.Models.Amphora, byte[]> dataStore;

        public AmphoraController(
            IEntityStore<Amphora.Common.Models.Amphora> amphoraEntityStore, 
            IDataStore<Amphora.Common.Models.Amphora, byte[]> dataStore)
        {
            this.amphoraEntityStore = amphoraEntityStore;
            this.dataStore = dataStore;
        }

        [HttpGet()]
        public IActionResult ListAmphora()
        {
            return Ok(this.amphoraEntityStore.List());
        }


        [HttpGet("{id}")]
        public IActionResult GetAmphoraInformation(string id)
        {
            return Ok(this.amphoraEntityStore.Get(id));
        }

        [HttpPut()]
        public IActionResult CreateAmphora([FromBody] Amphora.Common.Models.Amphora model)
        {
            if (model == null)
            {
                return BadRequest("Invalid Model");
            }
            return Ok(this.amphoraEntityStore.Set(model));
        }

        [HttpPost("{id}/fill")]
        public async Task<IActionResult> FillAmphora(string id)
        {
            var entity = amphoraEntityStore.Get(id);
            if (entity == null)
            {
                return BadRequest("Invalid Amphora Id");
            }

            dataStore.SetData(entity, await Request.Body.ReadFullyAsync());
            return Ok();
        }

        [HttpGet("{id}/drink")]
        public IActionResult DrinkAmphora(string id)
        {
            var entity = amphoraEntityStore.Get(id);
            if (entity == null)
            {
                return BadRequest("Invalid Amphora Id");
            }

            var data = dataStore.GetData(entity);
            return new FileContentResult(data, entity.ContentType);
        }
    }
}
