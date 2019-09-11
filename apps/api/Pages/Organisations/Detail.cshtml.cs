using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Organisations
{
    [Authorize]
    public class DetailModel : PageModel
    {
        private readonly IEntityStore<OrganisationModel> entityStore;
        private readonly IPermissionService permissionService;
        private readonly IUserManager userManager;

        public DetailModel(
            IEntityStore<OrganisationModel> entityStore,
            IPermissionService permissionService,
            IUserManager userManager)
        {
            this.entityStore = entityStore;
            this.permissionService = permissionService;
            this.userManager = userManager;
        }
        public OrganisationExtendedModel Organisation { get; set; }
        public bool CanEdit { get; private set; }
        public bool CanInvite { get; private set; }
        public bool CanAcceptInvite { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await userManager.GetUserAsync(User);
            if (string.IsNullOrEmpty(id))
            {
                this.Organisation = await entityStore.ReadAsync<OrganisationExtendedModel>(user.OrganisationId, user.OrganisationId);
            }
            else
            {
                this.Organisation = await entityStore.ReadAsync<OrganisationExtendedModel>(id, id);
            }
            if (this.Organisation == null) return RedirectToPage("/Index");

            this.CanEdit = await permissionService.IsAuthorizedAsync(user, this.Organisation, ResourcePermissions.Update);
            this.CanInvite = await permissionService.IsAuthorizedAsync(user, this.Organisation, ResourcePermissions.Create);
            if(this.Organisation.Invitations != null)
            {
                this.CanAcceptInvite = this.Organisation.Invitations.Any(i => string.Equals(i.TargetEmail.ToLower(), user.Email.ToLower()));
            }
            return Page();
        }
    }
}