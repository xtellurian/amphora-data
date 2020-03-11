using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Amphora.Api.Areas.Organisations.Pages
{
    [CommonAuthorize]
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
        public bool CanRequestToJoin { get; private set; }
        public bool CanRestrict { get; private set; }
        public bool CanInvite { get; private set; }
        public bool IsInOrg { get; private set; }
        public bool CanViewBalance { get; private set; }
        public IEnumerable<AmphoraModel> PinnedAmphorae { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await userService.ReadUserModelAsync(User);
            var appUser = await userService.UserStore.ReadAsync(user.Id);
            if (string.IsNullOrEmpty(id))
            {
                this.Organisation = await entityStore.ReadAsync(user.OrganisationId);
                if (this.Organisation == null) { return RedirectToPage("./Create"); }
            }
            else
            {
                this.Organisation = await entityStore.ReadAsync(id);
                if (this.Organisation == null) { return RedirectToPage("./Index"); }
            }

            this.IsInOrg = this.Organisation.IsInOrgansation(user);
            this.CanViewBalance = IsInOrg;
            this.CanEdit = await permissionService.IsAuthorizedAsync(user, this.Organisation, ResourcePermissions.Update);
            this.CanRestrict = CanEdit;
            this.CanInvite = await permissionService.IsAuthorizedAsync(user, this.Organisation, ResourcePermissions.Create);
            this.CanRequestToJoin = user.OrganisationId == null;
            // get pinned
            var query = amphoraeService.AmphoraStore.Query(a => a.OrganisationId == Organisation.Id);
            this.PinnedAmphorae = Organisation.PinnedAmphorae.AreAllNull()
                ? await query.Take(6).ToListAsync()
                : Organisation.PinnedAmphorae as IEnumerable<AmphoraModel>;
            return Page();
        }
    }
}