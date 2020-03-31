using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Platform;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Platform;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages
{
    public class InvitePageModel : OrganisationPageModel
    {
        private readonly IMapper mapper;
        private readonly IInvitationService invitationService;
        private readonly IPermissionService permissionService;

        public InvitePageModel(
            IOrganisationService organisationService,
            IMapper mapper,
            IInvitationService invitationService,
            IPermissionService permissionService,
            IUserDataService userDataService) : base(organisationService, userDataService)
        {
            this.mapper = mapper;
            this.invitationService = invitationService;
            this.permissionService = permissionService;
        }

        [BindProperty]
        public Invitation Input { get; set; }

        public async Task<IActionResult> OnGetAsync(string email = null)
        {
            if (await LoadPropertiesAsync())
            {
                if (email != null)
                {
                    Input ??= new Invitation();
                    Input.TargetEmail = email;
                }

                if (await permissionService.IsAuthorizedAsync(UserData, Organisation, ResourcePermissions.Create))
                {
                    return Page();
                }
                else
                {
                    return RedirectToPage("/Shared/Unauthorized", new { resourceId = Organisation.Id });
                }
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (await LoadPropertiesAsync())
            {
                if (!ModelState.IsValid)
                {
                    // return due to error
                    return Page();
                }

                // check email confirmed
                if (!User.IsEmailConfirmed())
                {
                    ModelState.AddModelError(string.Empty, "You haven't confirmed your email, so you can't invite anyone");
                    return Page();
                }

                if (Input?.TargetEmail != null)
                {
                    var model = mapper.Map<InvitationModel>(Input);
                    model.TargetOrganisationId = this.Organisation.Id;
                    model.TargetOrganisation = this.Organisation;
                    var op = await invitationService.CreateInvitation(User, model, true);
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
            else
            {
                return NotFound();
            }
        }
    }
}