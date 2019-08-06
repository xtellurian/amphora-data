using Amphora.Common.Models;
using Amphora.Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Schema;

namespace Amphora.Api.Api.Controllers
{
    [Route("api/schemas")]
    public class SchemaApiController : Controller
    {
        private readonly IEntityStore<Schema> store;

        public SchemaApiController(IEntityStore<Schema> store)
        {
            this.store = store;
        }


        [HttpPut]
        public IActionResult Create([FromBody] Schema schema)
        {
            if (schema == null) return BadRequest("Invalid Model");
            return Ok(store.Set(schema));
        }

        [HttpPut("JsonSchema")]
        public IActionResult CreateFromJsonSchema([FromBody] JSchema jSchema)
        {
            var schema = new Schema(jSchema);
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
