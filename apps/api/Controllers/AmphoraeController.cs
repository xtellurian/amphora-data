using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Common.Models.Amphorae;
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
        private readonly IAuthorizationService authorizationService;
        private readonly IUserManager userManager;
        private readonly IMapper mapper;

        public AmphoraeController(
            IAmphoraeService amphoraeService,
            IAmphoraFileService amphoraFileService,
            IAuthorizationService authorizationService,
            IUserManager userManager,
            IMapper mapper)
        {
            this.amphoraeService = amphoraeService;
            this.amphoraFileService = amphoraFileService;
            this.authorizationService = authorizationService;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        [HttpPost("api/amphorae")]
        public async Task<IActionResult> Create_Api([FromBody] AmphoraExtendedModel model)
        {
            if (model == null || model.Name == null)
            {
                return BadRequest("Invalid Model");
            }

            var result = await amphoraeService.CreateAsync<AmphoraExtendedModel>(User, model);
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
            var result = await this.amphoraeService.ReadAsync<AmphoraExtendedModel>(User, id);
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
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] AmphoraExtendedModel model)
        {
            var a = await this.amphoraeService.ReadAsync<AmphoraModel>(User, id);
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
            var readResult = await amphoraeService.ReadAsync<AmphoraModel>(User, id);
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

        [HttpGet("api/amphorae/{id}/files")]
        public async Task<IActionResult> ListFiles(string id)
        {
            var result = await amphoraeService.ReadAsync<AmphoraModel>(User, id);
            if (result.Succeeded)
            {
                var blobs = await amphoraFileService.Store.ListBlobsAsync(result.Entity);
                return Ok(blobs);
            }
            else if (result.WasForbidden)
            {
                return StatusCode(403, result.Message);
            }
            else
            {
                return NotFound("Amphora not found");
            }
        }

        [HttpGet("api/amphorae/{id}/files/{file}")]
        public async Task<IActionResult> DownloadFile(string id, string file)
        {
            var result = await amphoraeService.ReadAsync<AmphoraModel>(User, id);
            if (result.Succeeded)
            {
                var fileResult = await amphoraFileService.ReadFileAsync(User, result.Entity, file);
                if (fileResult.Succeeded)
                {
                    return File(fileResult.Entity, "application/octet-stream", file);
                }
                else if (fileResult.WasForbidden)
                {
                    return StatusCode(403, fileResult.Message);
                }
                else
                {
                    return BadRequest(fileResult.Message);
                }
            }
            else if (result.WasForbidden)
            {
                return StatusCode(403, result.Message);
            }
            else
            {
                return NotFound("Amphora not found");
            }
        }

        [HttpPut("api/amphorae/{id}/files/{file}")]
        public async Task<IActionResult> UploadToAmphora(string id, string file)
        {
            var result = await amphoraeService.ReadAsync<AmphoraModel>(User, id);
            if (result.Succeeded)
            {
                var fileResult = await amphoraFileService.WriteFileAsync(User, result.Entity, await Request.Body.ReadFullyAsync(), file);
                if (fileResult.Succeeded)
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
