using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos;
using Amphora.Api.Models.Dtos.Accounts.Memberships;
using Amphora.Common.Contracts;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Amphora.Api.Controllers.Accounts
{
    [ApiController]
    [SkipStatusCodePages]
    [Route("api/account/memberships")]
    [OpenApiTag("Account")]
    public class MembershipController : AccountControllerBase
    {
        private readonly IOrganisationService organisationService;
        private readonly IMapper mapper;

        public MembershipController(IOrganisationService organisationService,
                                    IUserDataService userDataService,
                                    IMapper mapper) : base(userDataService)
        {
            this.organisationService = organisationService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Returns a collection of members of an organisational account.
        /// </summary>
        /// <param name="id">Organisation Id. Defaults to your org.</param>
        /// <returns>A collection response of memberships.</returns>
        [HttpGet]
        [CommonAuthorize]
        [Produces(typeof(CollectionResponse<Membership>))]
        [ProducesBadRequest]
        public async Task<IActionResult> GetMemberships(string id)
        {
            var ensureRes = await EnsureIdAsync(id);
            if (ensureRes != null)
            {
                return ensureRes;
            }

            var orgRead = await organisationService.ReadAsync(User, OrganisationId);
            if (orgRead.Succeeded)
            {
                var memberships = mapper.Map<List<Membership>>(orgRead.Entity.Memberships);
                return Ok(new CollectionResponse<Membership>(memberships));
            }
            else
            {
                return Handle(orgRead);
            }
        }
    }
}