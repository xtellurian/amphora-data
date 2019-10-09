using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Amphorae.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUserService userService;
        private readonly ISearchService searchService;
        private readonly IAmphoraeService amphoraeService;
        private readonly IMapper mapper;

        public IndexModel(
            IUserService userService,
            ISearchService searchService,
            IAmphoraeService amphoraeService,
            IEntityStore<AmphoraModel> entityStore,
            IMapper mapper)
        {
            this.userService = userService;
            this.searchService = searchService;
            this.amphoraeService = amphoraeService;
            this.mapper = mapper;
            this.Amphorae = new List<AmphoraModel>();
        }

        public IEnumerable<AmphoraModel> Amphorae { get; set; }
        public string ViewType { get; private set; }

        public async Task<IActionResult> OnGetAsync(string viewType)
        {
            switch (viewType?.ToLower())
            {
                case "purchased":
                    ViewType = "purchased";
                    return await MyPurchasedAmphorae();
                case "org":
                    ViewType = "org";
                    return await OrgAmphorae();
                case "mine":
                default:
                    ViewType = "mine";
                    return await MyAmphorae();

            }
        }

        private async Task<IActionResult> MyPurchasedAmphorae()
        {
            var user = await this.userService.UserManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Account/Login", new { area = "Identity" });

            var x= await amphoraeService.AmphoraPurchasedBy(User, user);
            this.Amphorae = x; 
            return Page();
        }

            private async Task<IActionResult> OrgAmphorae()
        {
            var user = await this.userService.ReadUserModelAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            // get my amphora
            this.Amphorae = await amphoraeService.AmphoraStore.QueryAsync(a => a.OrganisationId == user.OrganisationId);
            return Page();
        }

        private async Task<IActionResult> MyAmphorae()
        {
            var user = await this.userService.UserManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            // get my amphora
            this.Amphorae = await amphoraeService.AmphoraStore.QueryAsync(a => a.CreatedById == user.Id);
            return Page();
        }
    }
}
