using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Organisations.Accounts;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
    }
}