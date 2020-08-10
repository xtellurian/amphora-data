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
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    [OpenApiTag("Organisations")]
    [OpenApiTag("Account")]
    [Route("api/Organisations/{id}/Account")] // backwards compat
    [Route("api/Account")]
    public class AccountController : AccountControllerBase
    {
        private readonly IOrganisationService organisationService;
        private readonly IMapper mapper;

        public AccountController(IOrganisationService organisationService,
                                 IUserDataService userDataService,
                                 IMapper mapper) : base(userDataService)
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
        [ProducesBadRequest]
        [CommonAuthorize]
        [HttpGet]
        public async Task<IActionResult> Read(string id)
        {
            var ensureRes = await EnsureIdAsync(id);
            if (ensureRes != null)
            {
                return ensureRes;
            }

            var res = await organisationService.ReadAsync(User, OrganisationId);
            if (res.Succeeded)
            {
                var dto = mapper.Map<Models.Dtos.Organisations.Account>(res.Entity.Account ?? new Account());
                return Ok(dto);
            }
            else
            {
                return Handle(res);
            }
        }
    }
}