using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Search;
using Amphora.Common.Contracts;
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
        private readonly IUserManager userManager;
        private readonly ISearchService searchService;
        private readonly IOrganisationService organisationService;

        public DetailModel(IUserManager userManager, ISearchService searchService, IOrganisationService organisationService)
        {
            this.userManager = userManager;
            this.searchService = searchService;
            this.organisationService = organisationService;
        }

        [BindProperty]
        public IApplicationUser AppUser { get; set; }
        public bool IsSelf {get; set; }
        public IEnumerable<AmphoraModel> PinnedAmphorae { get; private set; }
        public OrganisationModel Organisation { get; private set; }

        public async Task<IActionResult> OnGetAsync(string userName)
        {
            var user = await userManager.GetUserAsync(User);
            
            if( string.IsNullOrEmpty(userName))
            {
                this.AppUser = user;
                IsSelf = true;
            }
            else
            {
                var lookupUser = await userManager.FindByNameAsync(userName);
                IsSelf = lookupUser.Id == user.Id;
            }
            if (AppUser == null)
            {
                return RedirectToPage("./Missing");
            }
            var search = await searchService.SearchAmphora("", SearchParameters.ForUserAsCreator(user));
            this.PinnedAmphorae = search?.Results?.Select(a => a.Entity) ?? new List<AmphoraModel>() ;
            this.Organisation = await organisationService.Store.ReadAsync(user.OrganisationId, user.OrganisationId);
            return Page();
        }

    }
}