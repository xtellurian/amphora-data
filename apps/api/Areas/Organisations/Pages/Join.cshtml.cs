using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Platform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages
{
    [Authorize]
    public class JoinModel : PageModel
    {
        private readonly IInvitationService invitationService;

        public JoinModel(IInvitationService invitationService)
        {
            this.invitationService = invitationService;
        }

        public InvitationModel Invitation { get; private set; }

        [BindProperty]
        [Display(Name = "Accept Invitation")]
        public bool AcceptInvitation { get; set; }

        public async Task<IActionResult> OnGetAsync(string orgId)
        {
            if (orgId == null)
            {
                var res = await invitationService.GetMyInvitations(User);
                if (res.Succeeded)
                {
                    this.Invitation = res.Entity.FirstOrDefault(_ => !_.IsClaimed.HasValue || !_.IsClaimed.Value); // either null or false
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
                var res2 = await invitationService.GetInvitation(User, orgId);
                if (res2.Succeeded)
                {
                    this.Invitation = res2.Entity;
                }
                else if (res2.WasForbidden) { return StatusCode(403); }
                else { return BadRequest(); }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string invitationId)
        {
            if (invitationId == null) { throw new System.NullReferenceException("invitationId was null"); }
            if (!AcceptInvitation)
            {
                ModelState.AddModelError(string.Empty, "You must accept the invitation to continue.");
                return Page();
            }

            this.Invitation = await invitationService.Store.ReadAsync(invitationId);
            if (Invitation == null)
            {
                return BadRequest();
            }

            var result = await invitationService.AcceptInvitationAsync(User, Invitation);
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
    }
}