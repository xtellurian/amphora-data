using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Common.Models;
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
        private readonly IEntityStore<Organisation> entityStore;
        private readonly IOnboardingService onboardingService;

        public OrganisationsController(
            IOptionsMonitor<CreateOptions> options,
            IEntityStore<Organisation> entityStore,
            IOnboardingService onboardingService)
        {
            this.options = options;
            this.entityStore = entityStore;
            this.onboardingService = onboardingService;
        }

        [HttpGet("api/organisations")]
        public async Task<IActionResult> ListOrganisations()
        {
            var orgs = await entityStore.ListAsync();
            return Ok(orgs);
        }

        [HttpPost("api/organisations")]
        public async Task<IActionResult> CreateOrganisation([FromBody]Organisation org)
        {
            // check the key
            if (Request.Headers.ContainsKey("Create") && string.Equals(Request.Headers["Create"], options.CurrentValue.Key))
            {
                var result = await onboardingService.CreateOrganisationAsync(User, org);
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
        public async Task<IActionResult> UpdateOrganisation(string id, [FromBody]Organisation org)
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

        [HttpPost("api/organisations/{id}/invite/{email}")]
        public async Task<IActionResult> InviteToOrganisation(string id, string email)
        {
            var response = await onboardingService.InviteToOrganisation(User, email);
            if(response.Succeeded)
            {
                return Ok(response.Entity);
            }
            else if(response.WasForbidden)
            {
                return StatusCode(403, response.Message);
            }
            else
            {
                return BadRequest(response.Message);
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