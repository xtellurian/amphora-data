using Amphora.Common.Models;
using Amphora.Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Schema;
using System.Threading.Tasks;

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
        public async Task<IActionResult> CreateAsync([FromBody] Schema schema)
        {
            if (schema == null) return BadRequest("Invalid Model");
            return Ok(await store.SetAsync(schema));
        }

        [HttpPut("JsonSchema")]
        public async Task<IActionResult> CreateFromJsonSchemaAsync([FromBody] JSchema jSchema)
        {
            var schema = new Schema(jSchema);
            return Ok(await store.SetAsync(schema));
        }

        [HttpGet("{id}/JsonSchema")]
        public async Task<IActionResult> GetJsonSchemaAsync(string id) 
        {
            var schema = await store.GetAsync(id);
            if(schema == null) return NotFound();
            return Ok(schema.JsonSchema);
        }
    }
}
