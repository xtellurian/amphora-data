using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Platform;
using Amphora.Common.Models.Platform;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Amphora.Api.Controllers
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    [OpenApiIgnore]
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
        [HttpGet("api/invitations/")]
        [CommonAuthorize]
        public async Task<IActionResult> ReadMyInvitations()
        {
            var res = await invitationService.GetMyInvitations(User);
            if (res.Succeeded)
            {
                return Ok(res.Entity);
            }
            else if (res.WasForbidden) { return StatusCode(403); }
            else { return BadRequest(res.Message); }
        }

        /// <summary>
        /// Accepts or rejects an invitation sent to the user.
        /// </summary>
        /// <param name="orgId">Organisation to accept invitation for.</param>
        /// <param name="handle">Invitation information to accept or reject.</param>
        /// <returns> Invitation information that was submitted. </returns>
        [HttpPost("api/invitations/{orgId}")]
        [Produces(typeof(HandleInvitation))]
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
                    return BadRequest("The server does not know how to handle that request.");
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
        [CommonAuthorize]
        public async Task<IActionResult> InviteNewUser(Invitation invitation)
        {
            var model = mapper.Map<InvitationModel>(invitation);
            var res = await invitationService.CreateInvitation(User, model);
            if (res.Succeeded)
            {
                return Ok(invitation);
            }
            else if (res.WasForbidden) { return StatusCode(403); }
            else
            {
                return BadRequest(res.Message);
            }
        }
    }
}