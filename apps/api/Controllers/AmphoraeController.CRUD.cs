using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Models.Amphorae;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Amphora.Api.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public partial class AmphoraeController : Controller
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IAmphoraFileService amphoraFileService;
        private readonly IAuthorizationService authorizationService;
        private readonly IUserManager userManager;
        private readonly ISignalService signalService;
        private readonly IMapper mapper;

        public AmphoraeController(
            IAmphoraeService amphoraeService,
            IAmphoraFileService amphoraFileService,
            IAuthorizationService authorizationService,
            IUserManager userManager,
            ISignalService signalService,
            IMapper mapper)
        {
            this.amphoraeService = amphoraeService;
            this.amphoraFileService = amphoraFileService;
            this.authorizationService = authorizationService;
            this.userManager = userManager;
            this.signalService = signalService;
            this.mapper = mapper;
        }

        [HttpPost("api/amphorae")]
        public async Task<IActionResult> Create_Api([FromBody] AmphoraExtendedDto dto)
        {
            if (dto == null || dto.Name == null)
            {
                return BadRequest("Invalid Model");
            }
            var model = mapper.Map<AmphoraModel>(dto);
            model.IsPublic = true;
            var result = await amphoraeService.CreateAsync(User, model);
            if (result.Succeeded)
            {
                return Ok(mapper.Map<AmphoraExtendedDto>(result.Entity));
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
                return Ok(mapper.Map<AmphoraExtendedDto>(result.Entity));
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
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] AmphoraExtendedDto dto)
        {
            var a = await this.amphoraeService.ReadAsync(User, id);
            if (a == null) return NotFound();
            var model = mapper.Map<AmphoraModel>(dto);
            var result = await this.amphoraeService.UpdateAsync(User, model);
            if (result.Succeeded)
            {
                return Ok(mapper.Map<AmphoraExtendedDto>(result.Entity));
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
    }
}
