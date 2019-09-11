using System.Threading.Tasks;
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
        private readonly IAmphoraeService amphoraeService;
        private readonly IAmphoraFileService amphoraFileService;
        private readonly IDataStore<Common.Models.AmphoraModel, byte[]> dataStore;
        private readonly IAuthorizationService authorizationService;
        private readonly IUserManager userManager;
        private readonly IMapper mapper;

        public AmphoraeController(
            IAmphoraeService amphoraeService,
            IAmphoraFileService amphoraFileService,
            IDataStore<Amphora.Common.Models.AmphoraModel, byte[]> dataStore,
            IAuthorizationService authorizationService,
            IUserManager userManager,
            IMapper mapper)
        {
            this.amphoraeService = amphoraeService;
            this.amphoraFileService = amphoraFileService;
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
                var result = await amphoraeService.AmphoraStore.QueryAsync(a => a.GeoHash?.StartsWith(geoHash) ?? false);
                return Ok(result);
                //return Ok(await amphoraEntityStore.StartsWithQueryAsync("GeoHash", geoHash));
            }
            return Ok(await this.amphoraeService.AmphoraStore.ListAsync());
        }

        [HttpPost("api/amphorae")]
        public async Task<IActionResult> Create_Api([FromBody] Amphora.Common.Models.AmphoraModel model)
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
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] Amphora.Common.Models.AmphoraModel model)
        {
            var a = await this.amphoraeService.ReadAsync(User, id);
            if (a == null) return NotFound();

            var result = await this.amphoraeService.UpdateAsync(User, model);
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
                return BadRequest(result.Errors);
            }
        }

        [HttpDelete("api/amphorae/{id}")]
        public async Task<IActionResult> Delete_Api(string id)
        {
            var readResult = await amphoraeService.ReadAsync(User, id);
            if (!readResult.Succeeded) return NotFound();
            var result = await this.amphoraeService.DeleteAsync(User, readResult.Entity);
            if (result.Succeeded)
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

        [HttpGet("api/amphorae/{id}/files/{file}")]
        public async Task<IActionResult> DownloadFile(string id, string file)
        {
            var result = await amphoraeService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                var fileResult = await amphoraFileService.ReadFileAsync(User, result.Entity, file);
                if (fileResult.Succeeded)
                {
                    return File(fileResult.Entity, "application/octet-stream", file);
                }
                else
                {
                    return StatusCode(403, fileResult.Errors);
                }
            }
            else if(result.WasForbidden)
            {
                return StatusCode(403, result.Errors);
            }
            else
            {
                return NotFound("Amphora not found");
            }
        }

        [HttpPut("api/amphorae/{id}/files/{file}")]
        public async Task<IActionResult> UploadToAmphora(string id, string file)
        {
            var result = await amphoraeService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                var fileResult = await amphoraFileService.WriteFileAsync(User, result.Entity, await Request.Body.ReadFullyAsync(), file);
                if(fileResult.Succeeded)
                {
                    return Ok();
                }
                else if (result.WasForbidden)
                {
                    return StatusCode(403, result.Errors);
                }
                else
                {
                    return NotFound();
                }
            }
            else if (result.WasForbidden)
            {
                return StatusCode(403, result.Errors);
            }
            else
            {
                return NotFound();
            }
        }


    }
}
