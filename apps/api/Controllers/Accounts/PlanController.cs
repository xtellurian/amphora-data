using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations.Accounts;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Amphora.Api.Controllers.Accounts
{
    [ApiController]
    [SkipStatusCodePages]
    [OpenApiTag("Organisations")]
    [OpenApiTag("Account")]
    [Route("api/Account")]
    public class PlanController : AccountControllerBase
    {
        private readonly IOrganisationService organisationService;
        private readonly IMapper mapper;

        public PlanController(IOrganisationService organisationService,
                                 IUserDataService userDataService,
                                 IMapper mapper) : base(userDataService)
        {
            this.organisationService = organisationService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets an Organisation's plan information.
        /// </summary>
        /// <returns>The user's Organisation's plan. </returns>
        [Produces(typeof(Models.Dtos.Accounts.PlanInformation))]
        [ProducesBadRequest]
        [HttpGet("Plan")]
        [CommonAuthorize]
        public async Task<IActionResult> GetPlan()
        {
            var ensureRes = await EnsureIdAsync();
            if (ensureRes != null)
            {
                return ensureRes;
            }

            var res = await organisationService.ReadAsync(User, OrganisationId);
            if (res.Succeeded)
            {
                var p = res.Entity.Account.Plan.PlanType ?? Plan.PlanTypes.Free;
                var dto = new Models.Dtos.Accounts.PlanInformation()
                {
                    PlanType = p,
                    FriendlyName = p.ToString()
                };
                return Ok(dto);
            }
            else
            {
                return Handle(res);
            }
        }

        /// <summary>
        /// Set's an Organisation's plan.
        /// </summary>
        /// <param name="planType">The Plan Type. Should be PAYG or Glaze.</param>
        /// <returns>An Organisation's plan. </returns>
        [Produces(typeof(Models.Dtos.Accounts.PlanInformation))]
        [ProducesBadRequest]
        [HttpPost("Plan")]
        [CommonAuthorize]
        public async Task<IActionResult> SetPlan(string planType)
        {
            if (string.IsNullOrEmpty(planType))
            {
                return BadRequest("planType cannot be empty");
            }

            var ensureRes = await EnsureIdAsync();
            if (ensureRes != null)
            {
                return ensureRes;
            }

            var res = await organisationService.ReadAsync(User, OrganisationId);

            var plan = System.Enum.Parse(typeof(Plan.PlanTypes), planType) as Plan.PlanTypes? ?? Plan.PlanTypes.Free;
            if (res.Succeeded)
            {
                var org = res.Entity;
                org.Account ??= new Account();
                org.Account.Plan ??= new Plan();
                org.Account.Plan.PlanType = plan;
                var updateRes = await organisationService.UpdateAsync(User, org);
                if (updateRes.Succeeded)
                {
                    var dto = new Models.Dtos.Accounts.PlanInformation()
                    {
                        PlanType = plan,
                        FriendlyName = planType.ToString()
                    };
                    return Ok(dto);
                }
                else
                {
                    return Handle(updateRes);
                }
            }
            else
            {
                return Handle(res);
            }
        }
    }
}