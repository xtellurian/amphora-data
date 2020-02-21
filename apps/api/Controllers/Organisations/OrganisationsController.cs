using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Api.Options;
using Amphora.Common.Models.Organisations;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSwag.Annotations;

namespace Amphora.Api.Controllers
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    [OpenApiTag("Organisations")]
    public class OrganisationsController : Controller
    {
        private readonly IOptionsMonitor<CreateOptions> options;
        private readonly IMapper mapper;
        private readonly IEntityStore<OrganisationModel> entityStore;
        private readonly IOrganisationService organisationService;

        public OrganisationsController(
            IOptionsMonitor<CreateOptions> options,
            IMapper mapper,
            IEntityStore<OrganisationModel> entityStore,
            IOrganisationService organisationService)
        {
            this.options = options;
            this.mapper = mapper;
            this.entityStore = entityStore;
            this.organisationService = organisationService;
        }

        /// <summary>
        /// Creates a new Organisation. This will assign the logged in user to the organisation.
        /// </summary>
        /// <param name="org">Information of the new Organisation.</param>
        /// <returns> The Organisation metadata. </returns>
        [Produces(typeof(Organisation))]
        [HttpPost("api/organisations")]
        [CommonAuthorize]
        public async Task<IActionResult> Create([FromBody]Organisation org)
        {
            var model = mapper.Map<OrganisationModel>(org);
            var result = await organisationService.CreateAsync(User, model);
            if (result.Succeeded)
            {
                org = mapper.Map<Organisation>(result.Entity);
                return Ok(org);
            }
            else if (result.WasForbidden)
            {
                return StatusCode(403, result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        /// <summary>
        /// Updates an organisation.
        /// </summary>
        /// <param name="id">Organisation Id.</param>
        /// <param name="org">Organisation Information. All fields are updated.</param>
        /// <returns> The organisation metadaa. </returns>
        [HttpPut("api/organisations/{id}")]
        [CommonAuthorize]
        public async Task<IActionResult> Update(string id, [FromBody]Organisation org)
        {
            var entity = await entityStore.ReadAsync(id);
            if (entity == null) { return NotFound(); }
            entity.Name = org.Name;
            entity.About = org.About;
            entity.Address = org.Address;
            entity.WebsiteUrl = org.WebsiteUrl;
            var result = await entityStore.UpdateAsync(entity);
            var dto = mapper.Map<Organisation>(result);
            return Ok(dto);
        }

        /// <summary>
        /// Gets an organisation's details.
        /// </summary>
        /// <param name="id">Organisation Id.</param>
        /// <returns> The organisation metadata. </returns>
        [Produces(typeof(Organisation))]
        [HttpGet("api/organisations/{id}")]
        [CommonAuthorize]
        public async Task<IActionResult> Read(string id)
        {
            var org = await entityStore.ReadAsync(id);
            if (org == null) { return NotFound(); }
            else
            {
                var dto = mapper.Map<Organisation>(org);
                return Ok(dto);
            }
        }

        /// <summary>
        /// Deletes an organisation.
        /// </summary>
        /// <param name="id">Organisation Id.</param>
        /// <returns> A Message. </returns>
        [Produces(typeof(string))]
        [HttpDelete("api/organisations/{id}")]
        [CommonAuthorize]
        public async Task<IActionResult> Delete(string id)
        {
            var org = await entityStore.ReadAsync(id);
            if (org == null) { return NotFound(); }
            await entityStore.DeleteAsync(org);
            return Ok("Deleted Organisation");
        }
    }
}