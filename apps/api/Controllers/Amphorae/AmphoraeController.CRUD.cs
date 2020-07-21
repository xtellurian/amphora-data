using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using AutoMapper;
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
        [CommonAuthorize]
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
            else
            {
                return Handle(result);
            }
        }

        /// <summary>
        /// Gets a list of Amphora for yourself or your org, created or purchased by you (or organisation).
        /// </summary>
        /// <param name="scope">'self' or 'organisation'. Defaults to self.</param>
        /// <param name="accessType">'created' or 'purchased'. Defaults to created.</param>
        /// <param name="options">Options to control the response.</param>
        /// <returns>A list of Amphora.</returns>
        [Produces(typeof(IEnumerable<DetailedAmphora>))]
        [HttpGet]
        [CommonAuthorize]
        public async Task<IActionResult> List(string scope = "self", string accessType = "created", [FromQuery] ListAmphoraOptions options = null)
        {
            options ??= new ListAmphoraOptions(); // set defaults if not provided.
            if (!string.Equals(scope, "self") && !string.Equals(scope, "organisation"))
            {
                return BadRequest($"Parameter 'scope' must be 'self' or 'organisation', got {scope}");
            }

            if (!string.Equals(accessType, "created") && !string.Equals(accessType, "purchased"))
            {
                return BadRequest($"Parameter 'accessType' must be 'created' or 'purchased', got {accessType}");
            }

            var o = $"{scope?.ToLower()}.{accessType?.ToLower()}";
            switch (o)
            {
                case "self.created":
                    return Handle(await this.amphoraeService.ListForSelfAsync(User, options.Skip ?? 0, options.Take ?? 64, created: true, purchased: false));
                case "self.purchased":
                    return Handle(await this.amphoraeService.ListForSelfAsync(User, options.Skip ?? 0, options.Take ?? 64, created: false, purchased: true));
                case "organisation.created":
                    return Handle(await this.amphoraeService.ListForOrganisationAsync(User, options.Skip ?? 0, options.Take ?? 64, created: true, purchased: false));
                case "organisation.purchased":
                    return Handle(await this.amphoraeService.ListForOrganisationAsync(User, options.Skip ?? 0, options.Take ?? 64, created: false, purchased: true));
                default:
                    return BadRequest("Unknown Type of Get");
            }
        }

        /// <summary>
        /// Gets details of an Amphora by Id.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <returns>The Amphora details.</returns>
        [Produces(typeof(DetailedAmphora))]
        [HttpGet("{id}")]
        [CommonAuthorize]
        public async Task<IActionResult> Read(string id)
        {
            var result = await this.amphoraeService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                return Ok(mapper.Map<DetailedAmphora>(result.Entity));
            }
            else
            {
                return Handle(result);
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
        [CommonAuthorize]
        public async Task<IActionResult> Update(string id, [FromBody] EditAmphora amphora)
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
            else
            {
                return Handle(result);
            }
        }

        /// <summary>
        /// Deletes an Amphora.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <returns>A Message.</returns>
        [HttpDelete("{id}")]
        [CommonAuthorize]
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
            else
            {
                return Handle(result);
            }
        }

        private IActionResult Handle<T>(EntityOperationResult<IEnumerable<T>> result) where T : class
        {
            if (result.Succeeded)
            {
                return Ok(mapper.Map<List<DetailedAmphora>>(result.Entity));
            }
            else if (result.WasForbidden)
            {
                return StatusCode(403);
            }
            else
            {
                return NotFound(result.Message);
            }
        }

        private IActionResult Handle<T>(EntityOperationResult<T> result) where T : class
        {
            if (result.Succeeded)
            {
                return Ok(mapper.Map<List<DetailedAmphora>>(result.Entity));
            }
            else if (result.WasForbidden)
            {
                return StatusCode(403);
            }
            else
            {
                return NotFound(result.Message);
            }
        }
    }
}
