using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Search;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages
{
    [Authorize]
    public class DetailModel : PageModel
    {
        private readonly IEntityStore<OrganisationModel> entityStore;
        private readonly IAmphoraeService amphoraeService;
        private readonly IPermissionService permissionService;
        private readonly IUserService userService;

        public DetailModel(
            IEntityStore<OrganisationModel> entityStore,
            IAmphoraeService amphoraeService,
            IPermissionService permissionService,
            IUserService userService)
        {
            this.entityStore = entityStore;
            this.amphoraeService = amphoraeService;
            this.permissionService = permissionService;
            this.userService = userService;
        }
        public OrganisationModel Organisation { get; set; }
        public bool CanEdit { get; private set; }
        public bool CanInvite { get; private set; }
        public bool CanAcceptInvite { get; private set; }
        public bool CanViewMembers { get; private set; }
        public bool CanViewBalance { get; private set; }
        public IEnumerable<AmphoraModel> PinnedAmphorae { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await userService.ReadUserModelAsync(User);
            var appUser = await userService.UserManager.FindByIdAsync(user.Id);
            if (string.IsNullOrEmpty(id))
            {
                this.Organisation = await entityStore.ReadAsync(user.OrganisationId);
                if (this.Organisation == null) return RedirectToPage("./Create");
            }
            else
            {
                this.Organisation = await entityStore.ReadAsync(id);
                if (this.Organisation == null) return RedirectToPage("./Index");
            }

            this.CanViewMembers = this.Organisation.IsInOrgansation(user);
            this.CanViewBalance = CanViewMembers;
            this.CanEdit = await permissionService.IsAuthorizedAsync(user, this.Organisation, ResourcePermissions.Update);
            this.CanInvite = await permissionService.IsAuthorizedAsync(user, this.Organisation, ResourcePermissions.Create);
            if (this.Organisation.Invitations != null)
            {
                this.CanAcceptInvite = this.Organisation.Invitations.Any(i => string.Equals(i.TargetEmail.ToLower(), appUser.Email.ToLower()));
            }
            // get pinned
            var query = await amphoraeService.AmphoraStore.QueryAsync(a => a.OrganisationId == Organisation.Id);
            this.PinnedAmphorae = query.Take(6);
            return Page();
        }
    }
}