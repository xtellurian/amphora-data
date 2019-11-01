using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Platform;
using Amphora.Common.Models.Platform;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Controllers
{
    [ApiController]
    [SkipStatusCodePages]
    public class InvitationsController : Controller
    {
        private readonly IInvitationService invitationService;
        private readonly IMapper mapper;

        public InvitationsController(IInvitationService invitationService, IMapper mapper)
        {
            this.invitationService = invitationService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Returns all the invitations sent to me
        /// </summary>
        [HttpGet("api/invitations/")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetMyInvitations()
        {
            var res = await invitationService.GetMyInvitations(User);
            if (res.Succeeded)
            {
                return Ok(res.Entity);
            }
            else if (res.WasForbidden) return StatusCode(403);
            else return BadRequest(res.Message);
        }

        /// <summary>
        /// Accepts an invitation sent to me
        /// </summary>
        /// <param name="orgId">Organisation to accept invitation for</param>
        /// <param name="dto">Invitation to accept</param>
        [HttpPost("api/invitations/{orgId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AcceptInvitation(string orgId, AcceptInvitationDto dto)
        {
            var res = await invitationService.GetInvitation(User, dto.TargetOrganisationId);
            if (res.Succeeded)
            {
                var acceptResult = await invitationService.AcceptInvitationAsync(User, res.Entity);
                if(acceptResult.Succeeded)
                {
                    return Ok();
                }
                else if (acceptResult.WasForbidden) return StatusCode(403);
                else return BadRequest(acceptResult.Message);
            }
            else if (res.WasForbidden) return StatusCode(403);
            else return BadRequest(res.Message);

        }

        /// <summary>
        /// Invite a new email address to Amphora Data
        /// </summary>
        /// <param name="invitation">Invitation details</param>
        [HttpPost("api/invitations/")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> InviteNewUser(InvitationDto invitation)
        {
            var model = mapper.Map<InvitationModel>(invitation);
            var res = await invitationService.CreateInvitation(User, model);
            if (res.Succeeded)
            {
                return Ok();
            }
            else if (res.WasForbidden) return StatusCode(403);
            else
            {
                return BadRequest(res.Message);
            }
        }
    }
}