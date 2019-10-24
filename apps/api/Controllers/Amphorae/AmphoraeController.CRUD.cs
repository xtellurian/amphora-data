using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Models.Amphorae;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Amphora.Api.Controllers.Amphorae
{
    [ApiController]
    [Produces("application/json")]
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

        /// <summary>
        /// Creates a new empty Amphora in the user's organisation
        /// </summary>
        /// <param name="dto">Information for the new Amphora</param>  
        [HttpPost("api/amphorae")]
        [Produces(typeof(AmphoraExtendedDto))]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateAmphoraDto dto)
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
        /// <summary>
        /// Get's details of an Amphora by Id
        /// </summary>
        /// <param name="id">Amphora Id</param>  
        [Produces(typeof(AmphoraExtendedDto))]
        [HttpGet("api/amphorae/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        /// <summary>
        /// Updates the details of an Amphora by Id
        /// </summary>
        /// <param name="id">Amphora Id</param>  
        /// <param name="dto">Information to update</param>  
        [Produces(typeof(AmphoraExtendedDto))]
        [HttpPut("api/amphorae/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        /// <summary>
        /// Deletes an Amphora
        /// </summary>
        /// <param name="id">Amphora Id</param>  
        [HttpDelete("api/amphorae/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
