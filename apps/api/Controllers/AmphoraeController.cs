using System.Threading.Tasks;
using Amphora.Api.Authorization;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Amphora.Api.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AmphoraeController : Controller
    {
        private readonly IOrgScopedEntityStore<Common.Models.Amphora> amphoraEntityStore;
        private readonly IDataStore<Common.Models.Amphora, byte[]> dataStore;
        private readonly IAuthorizationService authorizationService;
        private readonly IMapper mapper;

        public AmphoraeController(
            IOrgScopedEntityStore<Amphora.Common.Models.Amphora> amphoraEntityStore,
            IDataStore<Amphora.Common.Models.Amphora, byte[]> dataStore,
            IAuthorizationService authorizationService,
            IMapper mapper)
        {
            this.amphoraEntityStore = amphoraEntityStore;
            this.dataStore = dataStore;
            this.authorizationService = authorizationService;
            this.mapper = mapper;
        }

        [HttpGet("api/amphorae")]
        public async Task<IActionResult> ListAmphoraAsync(string geoHash)
        {
            if (!string.IsNullOrEmpty(geoHash))
            {
                return Ok(await amphoraEntityStore.StartsWithQueryAsync("GeoHash", geoHash));
            }
            return Ok(await this.amphoraEntityStore.ListAsync());
        }

        [HttpGet("api/amphorae/{id}")]
        public async Task<IActionResult> ReadAsync(string id)
        {
            var a = await this.amphoraEntityStore.ReadAsync(id);
            var authorizationResult = await authorizationService
                .AuthorizeAsync(User, a, Operations.Read);
            if (a == null) return NotFound();
            return Ok(a);
        }

        [HttpPut("api/amphorae")]
        public async Task<IActionResult> Create_Api([FromBody] Amphora.Common.Models.Amphora model)
        {
            if (model == null || !model.IsValid())
            {
                return BadRequest("Invalid Model");
            }
            return Ok(await this.amphoraEntityStore.CreateAsync(model));
        }

        [HttpDelete("api/amphorae/{id}")]
        public async Task<IActionResult> Delete_Api(string id)
        {
            var entity = await amphoraEntityStore.ReadAsync(id);
            if (entity == null) return NotFound();
            await this.amphoraEntityStore.DeleteAsync(entity);
            return Ok();
        }

        [HttpPost("api/amphorae/{id}/upload")]
        public async Task<IActionResult> UploadToAmphora(string id, string name)
        {
            var entity = await amphoraEntityStore.ReadAsync(id);
            if (entity == null || string.IsNullOrEmpty(name))
            {
                return BadRequest("Invalid Amphora Id");
            }

            await dataStore.SetDataAsync(entity, await Request.Body.ReadFullyAsync(), name);
            return Ok();
        }

        [HttpGet("api/amphorae/{id}/download")]
        public async Task<IActionResult> Download(string id, string name)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();
            var entity = await amphoraEntityStore.ReadAsync(id);
            if (entity == null) return NotFound("Amphora not found");
            var data = await dataStore.GetDataAsync(entity, name);
            if (data == null) return NotFound("Data not found.");
            return File(data, "application/octet-stream", name);
        }
    }
}
