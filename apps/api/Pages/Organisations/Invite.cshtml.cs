using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Organisations
{
    public class InviteModel : PageModel
    {
        private readonly IOrganisationService organisationService;
        private readonly IPermissionService permissionService;
        private readonly IUserService userService;

        public InviteModel(
            IOrganisationService organisationService,
            IPermissionService permissionService,
            IUserService userService)
        {
            this.organisationService = organisationService;
            this.permissionService = permissionService;
            this.userService = userService;
        }
        public class InputModel
        {
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public OrganisationModel Organisation { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await userService.ReadUserModelAsync(User);
            if(id == null)
            {
                this.Organisation = await organisationService.Store.ReadAsync(user.OrganisationId);
            }
            else
            {
                this.Organisation = await organisationService.Store.ReadAsync(id);
            }

            if (Organisation == null) return RedirectToPage("./Detail");
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

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                ModelState.AddModelError(string.Empty, "Organisation Not Found");
                return Page();
            }
            var user = await userService.UserManager.GetUserAsync(User);
            var org = await organisationService.Store.ReadAsync(id);
            await organisationService.InviteToOrganisationAsync(User, id, Input.Email);
            return RedirectToPage("./Detail");
        }
    }
}