using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Api.Models.Dtos.Platform;
using Amphora.Api.Options;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Organisations;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSwag.Annotations;

namespace Amphora.Api.Controllers
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    [Route("api/Organisations")]
    [OpenApiTag("Organisations")]
    public class OrganisationsController : Controller
    {
        private readonly IOptionsMonitor<CreateOptions> options;
        private readonly IMapper mapper;
        private readonly IOrganisationService organisationService;

        public OrganisationsController(
            IOptionsMonitor<CreateOptions> options,
            IMapper mapper,
            IOrganisationService organisationService)
        {
            this.options = options;
            this.mapper = mapper;
            this.organisationService = organisationService;
        }

        /// <summary>
        /// Creates a new Organisation. This will assign the logged in user to the organisation.
        /// </summary>
        /// <param name="org">Information of the new Organisation.</param>
        /// <returns> The Organisation metadata. </returns>
        [Produces(typeof(Organisation))]
        [HttpPost]
        [CommonAuthorize]
        public async Task<IActionResult> Create([FromBody] Organisation org)
        {
            if (ModelState.IsValid)
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
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Updates an organisation.
        /// </summary>
        /// <param name="id">Organisation Id.</param>
        /// <param name="org">Organisation Information. All fields are updated.</param>
        /// <returns> The organisation metadaa. </returns>
        [HttpPut("{id}")]
        [CommonAuthorize]
        public async Task<IActionResult> Update(string id, [FromBody] Organisation org)
        {
            var userId = User.GetUserId();
            if (ModelState.IsValid)
            {
                var readRes = await organisationService.ReadAsync(User, id);
                if (readRes.Failed)
                {
                    return NotFound();
                }

                if (!readRes.Entity.IsAdministrator(userId))
                {
                    // not admin
                    return Unauthorized("User must be an administrator");
                }

                readRes.Entity.Name = org.Name;
                readRes.Entity.About = org.About;
                readRes.Entity.Address = org.Address;
                readRes.Entity.WebsiteUrl = org.WebsiteUrl;
                var result = await organisationService.UpdateAsync(User, readRes.Entity);
                if (result.Succeeded)
                {
                    var dto = mapper.Map<Organisation>(result.Entity);
                    return Ok(dto);
                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Gets an organisation's details.
        /// </summary>
        /// <param name="id">Organisation Id.</param>
        /// <returns> The organisation metadata. </returns>
        [Produces(typeof(Organisation))]
        [HttpGet("{id}")]
        [CommonAuthorize]
        public async Task<IActionResult> Read(string id)
        {
            var readRes = await organisationService.ReadAsync(User, id);
            if (readRes.Failed)
            {
                return NotFound();
            }
            else
            {
                var dto = mapper.Map<Organisation>(readRes.Entity);
                return Ok(dto);
            }
        }

        /// <summary>
        /// Gets an organisation's invitations.
        /// </summary>
        /// <param name="id">Organisation Id.</param>
        /// <returns> A list of invitations. </returns>
        [Produces(typeof(IEnumerable<Invitation>))]
        [HttpGet("{id}/Invitations")]
        [CommonAuthorize]
        public async Task<IActionResult> ReadInvitations(string id)
        {
            var readRes = await organisationService.ReadAsync(User, id);
            if (readRes.Failed)
            {
                return NotFound();
            }
            else
            {
                if (readRes.Entity.IsAdministrator(readRes.User))
                {
                    var dto = mapper.Map<List<Invitation>>(readRes.Entity.GlobalInvitations);
                    return Ok(dto);
                }
                else
                {
                    return BadRequest("You must be an organisation admin to read invitations");
                }
            }
        }

        /// <summary>
        /// Deletes an organisation.
        /// </summary>
        /// <param name="id">Organisation Id.</param>
        /// <returns> A Message. </returns>
        [Produces(typeof(string))]
        [HttpDelete("{id}")]
        [CommonAuthorize]
        public async Task<IActionResult> Delete(string id)
        {
            var readRes = await organisationService.ReadAsync(User, id);
            if (readRes.Failed)
            {
                return NotFound();
            }

            var deleteRes = await organisationService.DeleteAsync(User, readRes.Entity);
            if (deleteRes.Succeeded)
            {
                return Ok("Deleted Organisation");
            }
            else
            {
                return BadRequest(deleteRes.Message);
            }
        }
    }
}