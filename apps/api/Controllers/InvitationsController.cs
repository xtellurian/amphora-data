using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Platform;
using Amphora.Common.Models.Platform;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Amphora.Api.Controllers
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    [OpenApiIgnore]
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
        public async Task<IActionResult> ReadMyInvitations()
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
        [Produces(typeof(AcceptInvitationDto))]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AcceptInvitation(string orgId, AcceptInvitationDto dto)
        {
            var res = await invitationService.GetInvitation(User, dto.TargetOrganisationId);
            if (res.Succeeded)
            {
                var acceptResult = await invitationService.AcceptInvitationAsync(User, res.Entity);
                if(acceptResult.Succeeded)
                {
                    return Ok(dto);
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
        [Produces(typeof(InvitationDto))]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> InviteNewUser(InvitationDto invitation)
        {
            var model = mapper.Map<InvitationModel>(invitation);
            var res = await invitationService.CreateInvitation(User, model);
            if (res.Succeeded)
            {
                return Ok(invitation);
            }
            else if (res.WasForbidden) return StatusCode(403);
            else
            {
                return BadRequest(res.Message);
            }
        }
    }
}