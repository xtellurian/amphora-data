using api.Contracts;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Schema;

namespace api.Controllers
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
            return Ok(schema.JsonSchema);
        }
    }
}
