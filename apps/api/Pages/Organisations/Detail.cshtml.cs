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
        public OrganisationModel Organisation { get; set; }
        public bool CanEdit { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                var user = await userManager.GetUserAsync(User);
                this.Organisation = await entityStore.ReadAsync(user.OrganisationId);
                this.CanEdit = await permissionService.IsAuthorizedAsync(user, this.Organisation, ResourcePermissions.Update);
            }
            else
            {
                this.Organisation = await entityStore.ReadAsync(id);
            }
            if (this.Organisation == null) return RedirectToPage("/Index");
            return Page();
        }
    }
}