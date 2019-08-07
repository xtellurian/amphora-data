using Microsoft.AspNetCore.Mvc;
using Amphora.Common.Models;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using System.Threading.Tasks;

namespace Amphora.Api.Api.Controllers
{
    [Route("api/amphorae")]
    public class AmphoraApiController : Controller
    {
        private readonly IOrgEntityStore<Amphora.Common.Models.Amphora> amphoraEntityStore;
        private readonly IDataStore<Common.Models.Amphora, byte[]> dataStore;

        public AmphoraApiController(
            IOrgEntityStore<Amphora.Common.Models.Amphora> amphoraEntityStore, 
            IDataStore<Amphora.Common.Models.Amphora, byte[]> dataStore)
        {
            this.amphoraEntityStore = amphoraEntityStore;
            this.dataStore = dataStore;
        }

        [HttpGet()]
        public async Task<IActionResult> ListAmphoraAsync()
        {
            return Ok(await this.amphoraEntityStore.ListAsync());
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetAmphoraInformationAsync(string id)
        {
            return Ok(await this.amphoraEntityStore.GetAsync(id));
        }

        [HttpPut()]
        public async Task<IActionResult> Create([FromBody] Amphora.Common.Models.Amphora model)
        {
            if (model == null || !model.IsValid())
            {
                return BadRequest("Invalid Model");
            }
            return Ok(await this.amphoraEntityStore.SetAsync(model));
        }

        [HttpPost("{id}/upload")]
        public async Task<IActionResult> FillAmphora(string id)
        {
            var entity = await amphoraEntityStore.GetAsync(id);
            if (entity == null)
            {
                return BadRequest("Invalid Amphora Id");
            }

            dataStore.SetData(entity, await Request.Body.ReadFullyAsync());
            return Ok();
        }

        [HttpGet("{id}/drink")]
        public async Task<IActionResult> DrinkAmphoraAsync(string id)
        {
            var entity = await amphoraEntityStore.GetAsync(id);
            if (entity == null)
            {
                return BadRequest("Invalid Amphora Id");
            }

            var data = dataStore.GetData(entity);
            return new FileContentResult(data, entity.ContentType);
        }
    }
}
