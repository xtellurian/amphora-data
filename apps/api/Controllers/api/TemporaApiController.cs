using Microsoft.AspNetCore.Mvc;
using Amphora.Common.Models;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json;
using System;

namespace Amphora.Api.Api.Controllers
{
    [Route("api/temporae")]
    public class TemporaApiController : Controller
    {
        private readonly IDataEntityStore<Amphora.Common.Models.Tempora> temporaEntityStore;
        private readonly IEntityStore<Schema> schemaStore;
        private readonly IDataStore<Common.Models.Tempora, JObject> dataStore;

        public TemporaApiController(
            IDataEntityStore<Amphora.Common.Models.Tempora> temporaEntityStore, 
            IEntityStore<Schema> schemaStore,
            IDataStore<Amphora.Common.Models.Tempora, JObject> dataStore)
        {
            this.temporaEntityStore = temporaEntityStore;
            this.schemaStore = schemaStore;
            this.dataStore = dataStore;
        }

        [HttpGet()]
        public IActionResult ListTempora()
        {
            return Ok(this.temporaEntityStore.List());
        }


        [HttpGet("{id}")]
        public IActionResult GetInformation(string id)
        {
            return Ok(this.temporaEntityStore.Get(id));
        }

        [HttpPut()]
        public IActionResult CreateTempora([FromBody] Amphora.Common.Models.Tempora model)
        {
            if (model == null || !model.IsValid())
            {
                return BadRequest("Invalid Model");
            }
            return Ok(this.temporaEntityStore.Set(model));
        }

        [HttpPost("{id}/fill")]
        public async Task<IActionResult> FillTempora(string id, [FromBody] JObject jObj)
        {
            var entity = temporaEntityStore.Get(id);
            if (entity == null)
            {
                return BadRequest("Invalid Tempora Id");
            }

            // var schema = schemaStore.Get(entity.SchemaId);
            // if(schema == null)
            // {
            //     return BadRequest("Schema is null");
            // }
            // if (jObj.IsValid(schema.JSchema)) 
            // {
            //     return BadRequest("Invalid Payload");
            // }
            var jObjResult = dataStore.SetData(entity, jObj);
            return Ok(jObjResult);
        }

        [HttpGet("{id}/drink")]
        public IActionResult DrinkTempora(string id)
        {
            var entity = temporaEntityStore.Get(id);
            if (entity == null)
            {
                return BadRequest("Invalid Tempora Id");
            }

            throw new NotImplementedException();
        }
    }
}
