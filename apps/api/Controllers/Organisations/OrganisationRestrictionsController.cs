using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos;
using Amphora.Api.Models.Dtos.Permissions;
using Amphora.Common.Models.Permissions;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Amphora.Api.Controllers
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    [OpenApiTag("Organisations")]
    public class OrganisationRestrictionController : Controller
    {
        private readonly IOrganisationService organisationService;

        public OrganisationRestrictionController(IOrganisationService organisationService)
        {
            this.organisationService = organisationService;
        }

        /// <summary>
        /// Restricts an organisation from accessing data.
        /// </summary>
        /// <param name="id">Your organisation Id.</param>
        /// <param name="restriction">Restriction to create.</param>
        /// <returns> Simply 200. </returns>
        [Produces(typeof(Restriction))]
        [HttpPost("api/organisations/{id}/restrictions")]
        [CommonAuthorize]
        public async Task<IActionResult> Create(string id, Restriction restriction)
        {
            var readRes = await organisationService.ReadAsync(User, id);
            if (readRes.Succeeded)
            {
                var org = readRes.Entity;
                var targetOrg = await organisationService.Store.ReadAsync(restriction.TargetOrganisationId);
                if (targetOrg == null) { return BadRequest($"{restriction.TargetOrganisationId} is not an Organisation Id"); }
                if (org.Restrictions.Any(_ => _.TargetOrganisationId == restriction.TargetOrganisationId))
                {
                    return BadRequest("Restriction already exists on that organisation");
                }

                var entity = new RestrictionModel(readRes.User.Organisation, targetOrg, restriction.Kind);
                org.Restrictions.Add(entity);
                var updateRes = await organisationService.UpdateAsync(User, org);

                if (updateRes.Succeeded) { return Ok(new Restriction(entity.TargetOrganisationId, entity.Kind)); }
                else if (updateRes.WasForbidden) { return StatusCode(403); }
                else { return BadRequest(updateRes.Message); }
            }
            else if (readRes.WasForbidden) { return StatusCode(403); }
            else { return BadRequest(readRes.Message); }
        }

        /// <summary>
        /// Deletes a restriction.
        /// </summary>
        /// <param name="id">Your organisation Id.</param>
        /// <param name="targetOrganisationId">Organisation Id for which you want to delete a restriction.</param>
        /// <returns> Simply 200. </returns>
        [Produces(typeof(GenericResponse))]
        [HttpDelete("api/organisations/{id}/restrictions/{targetOrganisationId}")]
        [CommonAuthorize]
        public async Task<IActionResult> Delete(string id, string targetOrganisationId)
        {
            var readRes = await organisationService.ReadAsync(User, id);
            if (readRes.Succeeded)
            {
                var org = readRes.Entity;
                var restriction = org.Restrictions.FirstOrDefault(_ => _.TargetOrganisationId == targetOrganisationId);
                if (restriction == null) { return NotFound(new GenericResponse($"Restriction doesn't exist")); }

                org.Restrictions.Remove(restriction);
                var updateRes = await organisationService.UpdateAsync(User, org);

                if (updateRes.Succeeded) { return Ok(new GenericResponse("Restriction removed")); }
                else if (updateRes.WasForbidden) { return StatusCode(403, new GenericResponse(updateRes.Message)); }
                else { return BadRequest(new GenericResponse(updateRes.Message)); }
            }
            else if (readRes.WasForbidden) { return StatusCode(403, new GenericResponse(readRes.Message)); }
            else { return BadRequest(new GenericResponse(readRes.Message)); }
        }
    }
}