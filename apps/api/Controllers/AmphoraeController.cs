using System.Threading.Tasks;
using Amphora.Api.Authorization;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Api.Models;
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
        private readonly IEntityStore<Common.Models.Amphora> amphoraEntityStore;
        private readonly IAmphoraeService amphoraeService;
        private readonly IDataStore<Common.Models.Amphora, byte[]> dataStore;
        private readonly IAuthorizationService authorizationService;
        private readonly IUserManager userManager;
        private readonly IMapper mapper;

        public AmphoraeController(
            IEntityStore<Amphora.Common.Models.Amphora> amphoraEntityStore,
            IAmphoraeService amphoraeService,
            IDataStore<Amphora.Common.Models.Amphora, byte[]> dataStore,
            IAuthorizationService authorizationService,
            IUserManager userManager,
            IMapper mapper)
        {
            this.amphoraEntityStore = amphoraEntityStore;
            this.amphoraeService = amphoraeService;
            this.dataStore = dataStore;
            this.authorizationService = authorizationService;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        [HttpGet("api/amphorae")]
        public async Task<IActionResult> ListAmphoraAsync(string geoHash)
        {
            if (!string.IsNullOrEmpty(geoHash))
            {
                var result = await amphoraEntityStore.QueryAsync(a => a.GeoHash?.StartsWith(geoHash) ?? false);
                return Ok(result);
                //return Ok(await amphoraEntityStore.StartsWithQueryAsync("GeoHash", geoHash));
            }
            return Ok(await this.amphoraEntityStore.ListAsync());
        }

        [HttpPost("api/amphorae")]
        public async Task<IActionResult> Create_Api([FromBody] Amphora.Common.Models.Amphora model)
        {
            if (model == null || !model.IsValidDto())
            {
                return BadRequest("Invalid Model");
            }

            var result = await amphoraeService.CreateAsync(User, model);
            if (result.Succeeded)
            {
                return Ok(result.Entity);
            }
            else if (result.WasForbidden)
            {
                return StatusCode(403);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpGet("api/amphorae/{id}")]
        public async Task<IActionResult> ReadAsync(string id)
        {
            var result = await this.amphoraeService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                return Ok(result.Entity);
            }
            else if (result.WasForbidden)
            {
                return StatusCode(403);
            }
            else
            {
                return NotFound(result.Errors);
            }
        }

        [HttpPut("api/amphorae/{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] Amphora.Common.Models.Amphora model)
        {
            var a = await this.amphoraeService.ReadAsync(User, id);
            if (a == null) return NotFound();

            var result = await this.amphoraeService.UpdateAsync(User, model);
            if (result.Succeeded)
            {
                return Ok(result.Entity);
            }
            else if(result.WasForbidden)
            {
                return StatusCode(403);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpDelete("api/amphorae/{id}")]
        public async Task<IActionResult> Delete_Api(string id)
        {
            var readResult = await amphoraeService.ReadAsync(User, id);
            if (! readResult.Succeeded) return NotFound();
            var result = await this.amphoraeService.DeleteAsync(User, readResult.Entity);
            if(result.Succeeded)
            {
                return Ok();
            }
            else if (result.WasForbidden)
            {
                return StatusCode(403);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

// NEED TO ADD PERMISSIONS TO THESE
        
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
