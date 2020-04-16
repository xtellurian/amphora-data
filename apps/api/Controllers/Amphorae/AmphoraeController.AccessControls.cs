using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Permissions;
using Amphora.Common.Models.Permissions.Rules;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Amphora.Api.Controllers.Amphorae
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    [Route("api/amphorae/{id}/AccessControls")]
    [OpenApiTag("Amphorae")]
    public class AmphoraeAccessControlsController : EntityController
    {
        private readonly IAccessControlService accessControlService;
        private readonly IAmphoraeService amphoraeService;
        private readonly IOrganisationService organisationService;
        private readonly IUserDataService userDataService;
        private readonly IMapper mapper;

        public AmphoraeAccessControlsController(IAccessControlService accessControlService,
                                              IAmphoraeService amphoraeService,
                                              IOrganisationService organisationService,
                                              IUserDataService userDataService,
                                              IMapper mapper)
        {
            this.accessControlService = accessControlService;
            this.amphoraeService = amphoraeService;
            this.organisationService = organisationService;
            this.userDataService = userDataService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Creates a restriction on this Amphora.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <param name="rule">The rule to create.</param>
        /// <returns>The same restriction.</returns>
        [Produces(typeof(Models.Dtos.AccessControls.UserAccessRule))]
        [HttpPost("ForUser")]
        [CommonAuthorize]
        public async Task<IActionResult> CreateForUser(string id, [FromBody] Models.Dtos.AccessControls.UserAccessRule rule)
        {
            var readRes = await amphoraeService.ReadAsync(User, id);
            if (readRes.Succeeded)
            {
                // lookup the user
                var userToMakeRuleFor = await userDataService.ReadFromUsernameAsync(User, rule.Username);
                if (userToMakeRuleFor.Succeeded)
                {
                    var newRule = new UserAccessRule(rule.GetKind(), rule.Priority, userToMakeRuleFor.Entity);
                    var result = await accessControlService.CreateAsync(User, readRes.Entity, newRule);

                    if (result.Succeeded)
                    {
                        rule.Id = result.Entity.Id;
                        return Ok(rule);
                    }
                    else
                    {
                        return Handle(result);
                    }
                }
                else
                {
                    return BadRequest(userToMakeRuleFor.Message);
                }
            }
            else
            {
                return Handle(readRes);
            }
        }

        /// <summary>
        /// Creates a restriction on this Amphora.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <param name="rule">The rule to create.</param>
        /// <returns>The same restriction.</returns>
        [Produces(typeof(Models.Dtos.AccessControls.UserAccessRule))]
        [HttpPost("ForOrganisation")]
        [CommonAuthorize]
        public async Task<IActionResult> CreateForOrganisation(string id, [FromBody] Models.Dtos.AccessControls.OrganisationAccessRule rule)
        {
            var readRes = await amphoraeService.ReadAsync(User, id);
            if (readRes.Succeeded)
            {
                // lookup the org
                var orgToMakeRuleFor = await organisationService.ReadAsync(User, rule.OrganisationId);
                if (orgToMakeRuleFor.Succeeded)
                {
                    var newRule = new OrganisationAccessRule(rule.GetKind(), rule.Priority, orgToMakeRuleFor.Entity);
                    var result = await accessControlService.CreateAsync(User, readRes.Entity, newRule);

                    if (result.Succeeded)
                    {
                        rule.Id = result.Entity.Id;
                        return Ok(rule);
                    }
                    else
                    {
                        return Handle(result);
                    }
                }
                else
                {
                    return Handle(orgToMakeRuleFor);
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
        /// <param name="ruleId">The Id of the rule to delete.</param>
        /// <returns>An Empty 200.</returns>
        [HttpDelete("{ruleId}")]
        [CommonAuthorize]
        public async Task<IActionResult> Delete(string id, string ruleId)
        {
            if (id == null || ruleId == null)
            {
                return BadRequest();
            }

            var readRes = await amphoraeService.ReadAsync(User, id);
            if (readRes.Succeeded)
            {
                var deleteRes = await accessControlService.DeleteAsync(User, readRes.Entity, ruleId);
                return Handle(deleteRes);
            }
            else
            {
                return Handle(readRes);
            }
        }
    }
}