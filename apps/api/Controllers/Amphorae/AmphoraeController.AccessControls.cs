using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos;
using Amphora.Common.Contracts;
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
        /// Get's the list of access rules applied to users.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <returns>A list of rules.</returns>
        [Produces(typeof(IEnumerable<Models.Dtos.AccessControls.UserAccessRule>))]
        [ProducesBadRequest]
        [HttpGet("ForUser")]
        [CommonAuthorize]
        public async Task<IActionResult> GetUserRules(string id)
        {
            var readRes = await amphoraeService.ReadAsync(User, id);
            if (readRes.Succeeded)
            {
                var amphora = readRes.Entity;
                var res = new List<Models.Dtos.AccessControls.UserAccessRule>();
                foreach (var r in amphora.AccessControl.UserAccessRules)
                {
                    res.Add(new Models.Dtos.AccessControls.UserAccessRule
                    {
                        Priority = r.Priority ?? -1,
                        AllowOrDeny = r.Kind == Kind.Allow ? "Allow" : "Deny",
                        Id = r.Id,
                        Username = r.UserData.UserName
                    });
                }

                return Ok(res);
            }
            else
            {
                return Handle(readRes);
            }
        }

        /// <summary>
        /// Get's the list of access rules applied to organisations.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <returns>A list of rules.</returns>
        [ProducesBadRequest]
        [Produces(typeof(IEnumerable<Models.Dtos.AccessControls.OrganisationAccessRule>))]
        [HttpGet("ForOrganisation")]
        [CommonAuthorize]
        public async Task<IActionResult> GetOrganisationRules(string id)
        {
            var readRes = await amphoraeService.ReadAsync(User, id);
            if (readRes.Succeeded)
            {
                var amphora = readRes.Entity;
                var res = new List<Models.Dtos.AccessControls.OrganisationAccessRule>();
                foreach (var r in amphora.AccessControl.OrganisationAccessRules)
                {
                    res.Add(new Models.Dtos.AccessControls.OrganisationAccessRule
                    {
                        Priority = r.Priority ?? -1,
                        AllowOrDeny = r.Kind == Kind.Allow ? "Allow" : "Deny",
                        Id = r.Id,
                        OrganisationId = r.OrganisationId
                    });
                }

                return Ok(res);
            }
            else
            {
                return Handle(readRes);
            }
        }

        /// <summary>
        /// Get's the 'for all' rule, if it exists, else an empty 200.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <returns>A rule, if it exists.</returns>
        [Produces(typeof(Models.Dtos.AccessControls.AllAccessRule))]
        [ProducesBadRequest]
        [HttpGet("ForAll")]
        [CommonAuthorize]
        public async Task<IActionResult> GetForAllRule(string id)
        {
            var readRes = await amphoraeService.ReadAsync(User, id);
            if (readRes.Succeeded)
            {
                var amphora = readRes.Entity;
                var res = new List<Models.Dtos.AccessControls.AllAccessRule>();
                if (amphora.AccessControl.AllRule == null)
                {
                    return NotFound(new Response("Rule not found"));
                }
                else
                {
                    return Ok(new Models.Dtos.AccessControls.AllAccessRule
                    {
                        Id = amphora.AccessControl.AllRule.Id,
                        Priority = amphora.AccessControl.AllRule.Priority ?? 0,
                        AllowOrDeny = amphora.AccessControl.AllRule.Kind == Kind.Allow ? "Allow" : "Deny"
                    });
                }
            }
            else
            {
                return Handle(readRes);
            }
        }

        /// <summary>
        /// Creates an Access Control rule on this Amphora.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <param name="rule">The rule to create.</param>
        /// <returns>The rule.</returns>
        [Produces(typeof(Models.Dtos.AccessControls.UserAccessRule))]
        [ProducesBadRequest]
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
        /// Creates an Access Control Rule on this Amphora.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <param name="rule">The rule to create.</param>
        /// <returns>The same rule.</returns>
        [Produces(typeof(Models.Dtos.AccessControls.UserAccessRule))]
        [ProducesBadRequest]
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
        /// Creates an Access Control Rule for all on this Amphora.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <param name="rule">The rule to create.</param>
        /// <returns>The same rule.</returns>
        [Produces(typeof(Models.Dtos.AccessControls.AllAccessRule))]
        [ProducesBadRequest]
        [HttpPost("ForAll")]
        [CommonAuthorize]
        public async Task<IActionResult> CreateForAll(string id, [FromBody] Models.Dtos.AccessControls.AllAccessRule rule)
        {
            var readRes = await amphoraeService.ReadAsync(User, id);
            if (readRes.Succeeded)
            {
                var newRule = new AllRule(rule.GetKind(), rule.Priority);
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
                return Handle(readRes);
            }
        }

        /// <summary>
        /// Deletes an Access Control on this Amphora.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <param name="ruleId">The Id of the rule to delete.</param>
        /// <returns>An Empty 200.</returns>
        [HttpDelete("{ruleId}")]
        [Produces(typeof(Response))]
        [ProducesBadRequest]
        [CommonAuthorize]
        public async Task<IActionResult> Delete(string id, string ruleId)
        {
            if (id == null || ruleId == null)
            {
                return BadRequest(new Response("id or ruleId must not be null"));
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