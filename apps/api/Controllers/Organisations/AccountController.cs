using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Organisations.Accounts;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Amphora.Api.Controllers.Organisations
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    [OpenApiTag("Organisations")]
    [Route("api/Organisations/{id}/Account")]
    public class AccountController : Controller
    {
        private readonly IOrganisationService organisationService;
        private readonly IMapper mapper;

        public AccountController(IOrganisationService organisationService, IMapper mapper)
        {
            this.organisationService = organisationService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Get's an Organisation's account information.
        /// </summary>
        /// <param name="id">Organisation Id.</param>
        /// <returns>An Organisation's account metadata. </returns>
        [Produces(typeof(Models.Dtos.Organisations.Account))]
        [HttpGet]
        [CommonAuthorize]
        public async Task<IActionResult> Read(string id)
        {
            var res = await organisationService.ReadAsync(User, id);
            if (res.Succeeded)
            {
                var dto = mapper.Map<Models.Dtos.Organisations.Account>(res.Entity.Account ?? new Account());
                return Ok(dto);
            }
            else if (res.WasForbidden)
            {
                return StatusCode(403, res.Message);
            }
            else
            {
                return BadRequest(res.Message);
            }
        }

        /// <summary>
        /// Get's an Organisation's plan information.
        /// </summary>
        /// <param name="id">Organisation Id.</param>
        /// <returns>An Organisation's plan. </returns>
        [Produces(typeof(Models.Dtos.Organisations.PlanInformation))]
        [HttpGet("Plan")]
        [CommonAuthorize]
        public async Task<IActionResult> GetPlan(string id)
        {
            var res = await organisationService.ReadAsync(User, id);
            if (res.Succeeded)
            {
                var p = res.Entity.Account.Plan.PlanType ?? Plan.PlanTypes.Free;
                var dto = new Models.Dtos.Organisations.PlanInformation()
                {
                    PlanType = p,
                    FriendlyName = p.ToString()
                };
                return Ok(dto);
            }
            else if (res.WasForbidden)
            {
                return StatusCode(403, res.Message);
            }
            else
            {
                return BadRequest(res.Message);
            }
        }

        /// <summary>
        /// Set's an Organisation's plan.
        /// </summary>
        /// <param name="id">Organisation Id.</param>
        /// <param name="planType">The Plan Type.</param>
        /// <returns>An Organisation's plan. </returns>
        [Produces(typeof(Models.Dtos.Organisations.PlanInformation))]
        [HttpPost("Plan")]
        [CommonAuthorize]
        public async Task<IActionResult> SetPlan(string id, Plan.PlanTypes planType)
        {
            var res = await organisationService.ReadAsync(User, id);
            if (res.Succeeded)
            {
                var org = res.Entity;
                org.Account ??= new Account();
                org.Account.Plan ??= new Plan();
                org.Account.Plan.PlanType = planType;
                var updateRes = await organisationService.UpdateAsync(User, org);
                if (updateRes.Succeeded)
                {
                    var dto = new Models.Dtos.Organisations.PlanInformation()
                    {
                        PlanType = planType,
                        FriendlyName = planType.ToString()
                    };
                    return Ok(dto);
                }
                else
                {
                    return BadRequest(res.Message);
                }
            }
            else if (res.WasForbidden)
            {
                return StatusCode(403, res.Message);
            }
            else
            {
                return BadRequest(res.Message);
            }
        }
    }
}