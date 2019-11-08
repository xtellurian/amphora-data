using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Api.Options;
using Amphora.Common.Models.Organisations;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Controllers
{
    [ApiController]
     [SkipStatusCodePages]
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
        /// <param name="dto">Information of the new Organisation</param>  
        [Produces(typeof(OrganisationDto))]
        [HttpPost("api/organisations")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateOrganisation([FromBody]OrganisationDto dto) //TODO make this a DTO
        {
            var org = mapper.Map<OrganisationModel>(dto);
            var result = await organisationService.CreateOrganisationAsync(User, org);
            if (result.Succeeded)
            {
                dto = mapper.Map<OrganisationDto>(result.Entity);
                return Ok(dto);
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
        /// <param name="id">Organisation Id</param>  
        /// <param name="org">Organisation Information. All fields are updated.</param>  
        [HttpPut("api/organisations/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateOrganisation(string id, [FromBody]OrganisationDto org)
        {
            var entity = await entityStore.ReadAsync(id);
            if (entity == null) return NotFound();
            entity.Name = org.Name;
            entity.About = org.About;
            entity.Address = org.Address;
            entity.WebsiteUrl = org.WebsiteUrl;
            var result = await entityStore.UpdateAsync(entity);
            var dto = mapper.Map<OrganisationDto>(result);
            return Ok(dto);
        }
        /// <summary>
        /// Gets an organisation's details.
        /// </summary>
        /// <param name="id">Organisation Id</param>
        [Produces(typeof(OrganisationDto))]
        [HttpGet("api/organisations/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetOrganisation(string id)
        {
            var org = await entityStore.ReadAsync(id);
            if (org == null) return NotFound();
            else
            {
                var dto = mapper.Map<OrganisationDto>(org);
                return Ok(dto);
            }
        }

        /// <summary>
        /// Deletes an organisation.
        /// </summary>
        /// <param name="id">Organisation Id</param>
        [Produces(typeof(string))]
        [HttpDelete("api/organisations/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteOrganisation(string id)
        {
            var org = await entityStore.ReadAsync(id);
            if (org == null) return NotFound();
            await entityStore.DeleteAsync(org);
            return Ok("Deleted Organisation");
        }
    }
}