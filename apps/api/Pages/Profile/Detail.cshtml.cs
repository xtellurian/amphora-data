using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Users;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Profile
{
    [Authorize]
    public class DetailModel : PageModel
    {
        private readonly IUserService userService;
        private readonly IAmphoraeService amphoraeService;
        private readonly IOrganisationService organisationService;

        public DetailModel(IUserService userService, IAmphoraeService amphoraeService, IOrganisationService organisationService)
        {
            this.userService = userService;
            this.amphoraeService = amphoraeService;
            this.organisationService = organisationService;
        }

        public ApplicationUser AppUser { get; set; }

        public bool IsSelf {get; set; }
        public IEnumerable<AmphoraModel> PinnedAmphorae { get; private set; }
        public OrganisationModel Organisation { get; private set; }

        public async Task<IActionResult> OnGetAsync(string userName)
        {
            var user = await userService.UserManager.GetUserAsync(User);
            
            if( string.IsNullOrEmpty(userName))
            {
                this.AppUser = user;
                IsSelf = true;
            }
            else
            {
                var lookupUser = await userService.UserManager.FindByNameAsync(userName);
                IsSelf = lookupUser.Id == user.Id;
                this.AppUser = lookupUser;

            }
            if (AppUser == null)
            {
                return RedirectToPage("./Missing");
            }
            this.Organisation = await organisationService.Store.ReadAsync(AppUser.OrganisationId);
            var query = await amphoraeService.AmphoraStore.QueryAsync(a => a.CreatedBy == user.Id);
            this.PinnedAmphorae = query.Take(6);
            return Page();
        }

    }
}