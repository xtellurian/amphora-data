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
    [OpenApiTag("Account")]
    [Route("api/Account/Invitations")]
    public class AccountInvitationsController : AccountControllerBase
    {
        private readonly IOrganisationService organisationService;
        private readonly IMapper mapper;

        public AccountInvitationsController(IOrganisationService organisationService,
                                            IUserDataService userDataService,
                                            IMapper mapper) : base(userDataService)
        {
            this.organisationService = organisationService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets a list of invitations to the organisation.
        /// </summary>
        /// <param name="id">The organisation id.</param>
        /// <returns>The collection of invitations.</returns>
        [HttpGet]
        [CommonAuthorize]
        [Produces(typeof(CollectionResponse<Invitation>))]
        [ProducesBadRequest]
        public async Task<IActionResult> Invitations(string id)
        {
            var ensureRes = await EnsureIdAsync(id);
            if (ensureRes != null)
            {
                return ensureRes;
            }

            var orgRead = await organisationService.ReadAsync(User, OrganisationId);
            if (orgRead.Succeeded)
            {
                if (orgRead.Entity.IsAdministrator(orgRead.User))
                {
                    var dtos = mapper.Map<List<Invitation>>(orgRead.Entity.GlobalInvitations);
                    return Ok(new CollectionResponse<Invitation>(dtos));
                }
                else
                {
                    return BadRequest(new Response("You must be an admin to view invitations"));
                }
            }
            else
            {
                return Handle(orgRead);
            }
        }
    }
}