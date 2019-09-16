using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Common.Exceptions;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrganisationsController : Controller
    {
        private readonly IOptionsMonitor<CreateOptions> options;
        private readonly IEntityStore<OrganisationModel> entityStore;
        private readonly IOrganisationService organisationService;

        public OrganisationsController(
            IOptionsMonitor<CreateOptions> options,
            IEntityStore<OrganisationModel> entityStore,
            IOrganisationService organisationService)
        {
            this.options = options;
            this.entityStore = entityStore;
            this.organisationService = organisationService;
        }

        [HttpGet("api/organisations")]
        public async Task<IActionResult> ListOrganisations()
        {
            var orgs = await entityStore.ListAsync();
            return Ok(orgs);
        }

        [HttpPost("api/organisations")]
        public async Task<IActionResult> CreateOrganisation([FromBody]OrganisationExtendedModel org)
        {
            // check the key
            if (Request.Headers.ContainsKey("Create") && string.Equals(Request.Headers["Create"], options.CurrentValue.Key))
            {
                var result = await organisationService.CreateOrganisationAsync(User, org);
                if (result.Succeeded)
                {
                    return Ok(result.Entity);
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
                return Unauthorized("Create Key Required");
            }
        }

        [HttpPut("api/organisations/{id}")]
        public async Task<IActionResult> UpdateOrganisation(string id, [FromBody]OrganisationModel org)
        {
            var entity = await entityStore.ReadAsync(id);
            if (entity == null) return NotFound();
            org.Id = entity.Id;
            org.OrganisationId = entity.OrganisationId;
            var result = await entityStore.UpdateAsync(org);
            return Ok(result);
        }

        [HttpGet("api/organisations/{id}")]
        public async Task<IActionResult> GetOrganisation(string id)
        {
            var org = await entityStore.ReadAsync(id);
            if (org == null) return NotFound();
            else return Ok(org);
        }


        [HttpPost("api/organisations/{id}/invitations/")]
        public async Task<IActionResult> InviteToOrganisation(string id, [FromBody] Invitation invitation)
        {
            try
            {
                await organisationService.InviteToOrganisationAsync(User, id, invitation.TargetEmail);
                return Ok();
            }
            catch (PermissionDeniedException permEx)
            {
                return StatusCode(403, permEx.Message);
            }
        }

        [HttpGet("api/organisations/{id}/invitations/")]
        public async Task<IActionResult> AcceptInvitation(string id)
        {
            try
            {
                await organisationService.AcceptInvitation(User, id);
                return Ok();
            }
            catch (PermissionDeniedException permEx)
            {
                return StatusCode(403, permEx.Message);
            }
        }



        [HttpDelete("api/organisations/{id}")]
        public async Task<IActionResult> DeleteOrganisation(string id)
        {
            var org = await entityStore.ReadAsync(id);
            if (org == null) return NotFound();
            await entityStore.DeleteAsync(org);
            return Ok();
        }
    }
}