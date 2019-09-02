using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrganisationsController : Controller
    {
        private readonly IEntityStore<Organisation> entityStore;

        public OrganisationsController(IEntityStore<Organisation> entityStore)
        {
            this.entityStore = entityStore;
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
            var result = await entityStore.CreateAsync(org);
            return Ok(result);
        }

        [HttpPut("api/organisations/{id}")]
        public async Task<IActionResult> CreateOrganisation(string id, [FromBody]Organisation org)
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