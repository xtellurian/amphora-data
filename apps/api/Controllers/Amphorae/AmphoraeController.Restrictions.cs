using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Permissions;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Permissions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Amphora.Api.Controllers.Amphorae
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    [Route("api/amphorae/{id}/restrictions")]
    [OpenApiTag("Amphorae")]
    public class AmphoraeRestrictionsController : EntityController
    {
        private readonly IRestrictionService restrictionService;
        private readonly IAmphoraeService amphoraeService;
        private readonly IEntityStore<OrganisationModel> orgStore;
        private readonly IMapper mapper;

        public AmphoraeRestrictionsController(IRestrictionService restrictionService,
                                              IAmphoraeService amphoraeService,
                                              IEntityStore<OrganisationModel> orgStore,
                                              IMapper mapper)
        {
            this.restrictionService = restrictionService;
            this.amphoraeService = amphoraeService;
            this.orgStore = orgStore;
            this.mapper = mapper;
        }

        /// <summary>
        /// Creates a restriction on this Amphora.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <param name="restriction">The restriction to create.</param>
        /// <returns>The same restriction.</returns>
        [Produces(typeof(Restriction))]
        [HttpPost]
        [CommonAuthorize]
        public async Task<IActionResult> Create(string id, [FromBody] Restriction restriction)
        {
            var readRes = await amphoraeService.ReadAsync(User, id);
            if (readRes.Succeeded)
            {
                var org = await orgStore.ReadAsync(restriction.TargetOrganisationId);
                if (org == null)
                {
                    return BadRequest($"Target Organisation Id {restriction.TargetOrganisationId} doesn't exist.");
                }

                var model = new RestrictionModel(readRes.Entity, org, restriction.Kind);
                var result = await restrictionService.CreateAsync(User, model);
                if (result.Succeeded)
                {
                    restriction.Id = result.Entity.Id;
                    return Ok(restriction);
                }
                else
                {
                    return Handle(result);
                }
            }
            else
            {
                return Handle(readRes);
            }
        }

        /// <summary>
        /// Deletes a restriction on this Amphora.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <param name="restrictionId">The Id of the restriction to delete.</param>
        /// <returns>An Empty 200.</returns>
        [Produces(typeof(Restriction))]
        [HttpDelete("{restrictionId}")]
        [CommonAuthorize]
        public async Task<IActionResult> Delete(string id, string restrictionId)
        {
            if (id == null || restrictionId == null)
            {
                return BadRequest();
            }

            var readRes = await amphoraeService.ReadAsync(User, id);
            if (readRes.Succeeded)
            {
                var result = await restrictionService.DeleteAsync(User, restrictionId);
                if (result.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    return Handle(result);
                }
            }
            else
            {
                return Handle(readRes);
            }
        }
    }
}