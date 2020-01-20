using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Platform;
using Amphora.Common.Models;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Platform;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages
{
    public class InvitePageModel : PageModel
    {
        private readonly IOrganisationService organisationService;
        private readonly IMapper mapper;
        private readonly IInvitationService invitationService;
        private readonly IPermissionService permissionService;
        private readonly IUserService userService;

        public InvitePageModel(
            IOrganisationService organisationService,
            IMapper mapper,
            IInvitationService invitationService,
            IPermissionService permissionService,
            IUserService userService)
        {
            this.organisationService = organisationService;
            this.mapper = mapper;
            this.invitationService = invitationService;
            this.permissionService = permissionService;
            this.userService = userService;
        }

        [BindProperty]
        public InvitationDto Input { get; set; }
        [BindProperty]
        [Display(Name = "Invite to my Organisation")]
        public bool InviteToOrganisation { get; set; }
        public OrganisationModel Organisation { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userService.ReadUserModelAsync(User);
            this.Organisation = user.Organisation;

            if (Organisation == null) { return RedirectToPage("./Detail"); }
            var authorized = await permissionService.IsAuthorizedAsync(user, Organisation, ResourcePermissions.Create);
            if (authorized)
            {
                return Page();
            }
            else
            {
                return RedirectToPage("/Shared/Unauthorized", new { resourceId = Organisation.Id });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await userService.ReadUserModelAsync(User);
            this.Organisation = user.Organisation;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // check email confirmed
            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "You must have a confirmed email address");
                return Page();
            }

            if (Input?.TargetEmail != null)
            {
                var model = mapper.Map<InvitationModel>(Input);
                model.TargetOrganisationId = this.Organisation.Id;
                model.TargetOrganisation = this.Organisation;
                var op = await invitationService.CreateInvitation(User, model, InviteToOrganisation);
                if (op.Succeeded)
                {
                    return RedirectToPage("./Detail");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, op.Message);
                    return Page();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Email required");
                return Page();
            }
        }
    }
}