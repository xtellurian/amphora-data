using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos;
using Amphora.Api.Models.Dtos.Accounts.Memberships;
using Amphora.Api.Models.Dtos.Platform;
using Amphora.Common.Models.Platform;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

// invitations should not go inside /account
// because invitations exist outside of an account
// i.e. you can be invited into another's account

namespace Amphora.Api.Controllers
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    public class InvitationsController : EntityController
    {
        private readonly IInvitationService invitationService;
        private readonly IMapper mapper;

        public InvitationsController(IInvitationService invitationService, IMapper mapper)
        {
            this.invitationService = invitationService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Returns all the invitations sent to me.
        /// </summary>
        /// <returns> A collection of invitations.</returns>
        [Produces(typeof(IEnumerable<Invitation>))]
        [ProducesBadRequest]
        [CommonAuthorize]
        [HttpGet("api/invitations/")]
        public async Task<IActionResult> ReadMyInvitations()
        {
            var res = await invitationService.GetMyInvitations(User);
            if (res.Succeeded)
            {
                var dto = mapper.Map<List<Invitation>>(res.Entity);
                return Ok(dto);
            }
            else
            {
                return Handle(res);
            }
        }

        /// <summary>
        /// Accepts or rejects an invitation sent to the user.
        /// </summary>
        /// <param name="orgId">Organisation to accept invitation for.</param>
        /// <param name="handle">Invitation information to accept or reject.</param>
        /// <returns> Invitation information that was submitted. </returns>
        [HttpPost("api/invitations/{orgId}")]
        [Produces(typeof(HandleInvitation))]
        [ProducesBadRequest]
        [CommonAuthorize]
        public async Task<IActionResult> AcceptInvitation(string orgId, HandleInvitation handle)
        {
            var res = await invitationService.GetInvitation(User, handle.TargetOrganisationId);
            if (res.Succeeded)
            {
                if (handle.AcceptOrReject == true)
                {
                    var acceptResult = await invitationService.HandleInvitationAsync(User, res.Entity, InvitationTrigger.Accept);
                    if (acceptResult.Succeeded)
                    {
                        return Ok(handle);
                    }
                    else
                    {
                        return Handle(acceptResult);
                    }
                }
                else if (handle.AcceptOrReject == false)
                {
                    var rejectResult = await invitationService.HandleInvitationAsync(User, res.Entity, InvitationTrigger.Reject);
                    if (rejectResult.Succeeded)
                    {
                        return Ok(handle);
                    }
                    else
                    {
                        return Handle(rejectResult);
                    }
                }
                else
                {
                    return BadRequest(new Response("The server does not know how to handle that request."));
                }
            }
            else
            {
                return Handle(res);
            }
        }

        /// <summary>
        /// Invite a new email address to Amphora Data.
        /// </summary>
        /// <param name="invitation">Invitation details.</param>
        /// <returns> An Invitation Object. </returns>
        [HttpPost("api/invitations/")]
        [Produces(typeof(Invitation))]
        [ProducesBadRequest]
        [CommonAuthorize]
        public async Task<IActionResult> InviteNewUser(Invitation invitation)
        {
            var model = mapper.Map<InvitationModel>(invitation);
            var res = await invitationService.CreateInvitation(User, model);
            if (res.Succeeded)
            {
                return Ok(invitation);
            }
            else
            {
                return Handle(res);
            }
        }
    }
}