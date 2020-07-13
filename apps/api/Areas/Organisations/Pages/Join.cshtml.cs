using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Platform;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages
{
    [CommonAuthorize]
    public class JoinModel : PageModel
    {
        private readonly IInvitationService invitationService;

        public JoinModel(IInvitationService invitationService)
        {
            this.invitationService = invitationService;
        }

        public InvitationModel Invitation { get; private set; }
        public IUser UserData { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                var res = await invitationService.GetMyInvitations(User);
                if (res.Succeeded)
                {
                    this.Invitation = res.Entity.FirstOrDefault(_ => _.State == InvitationState.Open);
                    this.UserData = res.User;
                }
                else if (res.WasForbidden) { return StatusCode(403); }
                else
                {
                    ModelState.AddModelError(string.Empty, res.Message);
                    return Page();
                }
            }
            else
            {
                var res2 = await invitationService.GetInvitation(User, id);
                if (res2.Succeeded)
                {
                    this.Invitation = res2.Entity;
                    this.UserData = res2.User;
                }
                else if (res2.WasForbidden) { return StatusCode(403); }
                else { return BadRequest(); }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAcceptAsync(string invitationId)
        {
            if (invitationId == null) { throw new System.NullReferenceException("invitationId was null"); }

            this.Invitation = await invitationService.Store.ReadAsync(invitationId);
            if (Invitation == null)
            {
                return BadRequest();
            }

            var result = await invitationService.HandleInvitationAsync(User, Invitation, InvitationTrigger.Accept);
            if (result.Succeeded)
            {
                return RedirectToPage("./Detail", new { id = Invitation.TargetOrganisationId });
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return Page();
            }
        }

        public async Task<IActionResult> OnPostRejectAsync(string invitationId)
        {
            if (invitationId == null) { throw new System.NullReferenceException("invitationId was null"); }

            this.Invitation = await invitationService.Store.ReadAsync(invitationId);
            if (Invitation == null)
            {
                return BadRequest();
            }

            var result = await invitationService.HandleInvitationAsync(User, Invitation, InvitationTrigger.Reject);

            if (result.Succeeded)
            {
                return RedirectToPage("./Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return Page();
            }
        }
    }
}