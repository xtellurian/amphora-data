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

namespace Amphora.Api.Pages.Organisations
{
    [Authorize]
    public class DetailModel : PageModel
    {
        private readonly IEntityStore<OrganisationModel> entityStore;
        private readonly ISearchService searchService;
        private readonly IPermissionService permissionService;
        private readonly IUserService userService;

        public DetailModel(
            IEntityStore<OrganisationModel> entityStore,
            ISearchService searchService,
            IPermissionService permissionService,
            IUserService userService)
        {
            this.entityStore = entityStore;
            this.searchService = searchService;
            this.permissionService = permissionService;
            this.userService = userService;
        }
        public OrganisationExtendedModel Organisation { get; set; }
        public bool CanEdit { get; private set; }
        public bool CanInvite { get; private set; }
        public bool CanAcceptInvite { get; private set; }
        public bool CanViewMembers { get; private set; }
        public IEnumerable<AmphoraModel> PinnedAmphorae { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await userService.UserManager.GetUserAsync(User);
            if (string.IsNullOrEmpty(id))
            {
                this.Organisation = await entityStore.ReadAsync<OrganisationExtendedModel>(user.OrganisationId, user.OrganisationId);
            }
            else
            {
                this.Organisation = await entityStore.ReadAsync<OrganisationExtendedModel>(id, id);
            }
            if (this.Organisation == null) return RedirectToPage("/Index");

            this.CanViewMembers = this.Organisation.IsInOrgansation(user);

            this.CanEdit = await permissionService.IsAuthorizedAsync(user, this.Organisation, ResourcePermissions.Update);
            this.CanInvite = await permissionService.IsAuthorizedAsync(user, this.Organisation, ResourcePermissions.Create);
            if (this.Organisation.Invitations != null)
            {
                this.CanAcceptInvite = this.Organisation.Invitations.Any(i => string.Equals(i.TargetEmail.ToLower(), user.Email.ToLower()));
            }
            // get pinned
            var searchResults = await searchService.SearchAmphora("", SearchParameters.ByOrganisation(id));
            this.PinnedAmphorae = searchResults?.Results?.Select(a => a.Entity) ?? new List<AmphoraModel>();
            return Page();
        }
    }
}