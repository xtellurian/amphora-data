using Amphora.Common.Models;
using Amphora.Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Schema;

namespace Amphora.Api.Api.Controllers
{
    [Route("api/schemas")]
    public class AmphoraSchemaController : Controller
    {
        private readonly IAmphoraEntityStore<AmphoraSchema> store;

        public AmphoraSchemaController(IAmphoraEntityStore<AmphoraSchema> store)
        {
            this.store = store;
        }


        [HttpPut]
        public IActionResult Create([FromBody] AmphoraSchema schema)
        {
            if (schema == null) return BadRequest("Invalid Model");
            return Ok(store.Set(schema));
        }

        [HttpPut("JsonSchema")]
        public IActionResult CreateFromJsonSchema([FromBody] JSchema jSchema)
        {
            var schema = new AmphoraSchema(jSchema);
            return Ok(store.Set(schema));
        }

        [HttpGet("{id}/JsonSchema")]
        public IActionResult GetJsonSchema(string id) 
        {
            var schema = store.Get(id);
            if(schema == null) return NotFound();
            return Ok(schema.JsonSchema);
        }
    }
}
