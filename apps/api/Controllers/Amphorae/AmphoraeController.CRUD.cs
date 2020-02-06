using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Models.Amphorae;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Amphora.Api.Controllers.Amphorae
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    [Produces("application/json")]
    [Route("api/amphorae")]
    [OpenApiTag("Amphorae")]
    public class AmphoraeController : Controller
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IAuthorizationService authorizationService;
        private readonly IMapper mapper;

        public AmphoraeController(
            IAmphoraeService amphoraeService,
            IAuthorizationService authorizationService,
            IMapper mapper)
        {
            this.amphoraeService = amphoraeService;
            this.authorizationService = authorizationService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Creates a new empty Amphora in the user's organisation.
        /// </summary>
        /// <param name="amphora">Information for the new Amphora.</param>
        /// <returns>A new Amphora.</returns>
        [HttpPost]
        [Produces(typeof(DetailedAmphora))]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Create([FromBody] CreateAmphora amphora)
        {
            if (amphora == null || amphora.Name == null)
            {
                return BadRequest("Invalid Model");
            }

            var model = mapper.Map<AmphoraModel>(amphora);
            model.IsPublic = true;
            var result = await amphoraeService.CreateAsync(User, model);
            if (result.Succeeded)
            {
                return Ok(mapper.Map<DetailedAmphora>(result.Entity));
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
        /// Get's details of an Amphora by Id.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <returns>The Amphora details.</returns>
        [Produces(typeof(DetailedAmphora))]
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Read(string id)
        {
            var result = await this.amphoraeService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                return Ok(mapper.Map<DetailedAmphora>(result.Entity));
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
        /// Updates the details of an Amphora by Id.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <param name="amphora">Information to update. Nulls are NOT ignored.</param>
        /// <returns>The Amphora details.</returns>
        [Produces(typeof(DetailedAmphora))]
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Update(string id, [FromBody] DetailedAmphora amphora)
        {
            var a = await this.amphoraeService.ReadAsync(User, id);
            if (a == null) { return NotFound(); }
            var model = await this.amphoraeService.ReadAsync(User, id);
            var entity = model.Entity.UpdateProperties(amphora);
            var result = await this.amphoraeService.UpdateAsync(User, entity);
            if (result.Succeeded)
            {
                return Ok(mapper.Map<DetailedAmphora>(result.Entity));
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
        /// Deletes an Amphora.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <returns>A Message.</returns>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Produces(typeof(string))]
        public async Task<IActionResult> Delete(string id)
        {
            var readResult = await amphoraeService.ReadAsync(User, id);
            if (!readResult.Succeeded) { return NotFound(); }
            var result = await this.amphoraeService.DeleteAsync(User, readResult.Entity);
            if (result.Succeeded)
            {
                return Ok("Deleted Amohora");
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
