using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Search;
using Amphora.Common.Models.Amphorae;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Amphorae
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
        [BindProperty(SupportsGet = true)]
        public string ViewType { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            switch (ViewType?.ToLower())
            {
                case "purchased":
                    return await MyPurchasedAmphorae();
                case "org":
                    return await OrgAmphorae();
                case "mine":
                default:
                    return await MyAmphorae();

            }
        }

        private async Task<IActionResult> MyPurchasedAmphorae()
        {
            var user = await this.userService.UserManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Account/Login", new { area = "Identity" });
            var result = await searchService.SearchAmphora(string.Empty, SearchParameters.AllPurchased(user.Id));
            this.Amphorae = result.Results.Select(r => r.Entity);
            return Page();
        }

        private async Task<IActionResult> OrgAmphorae()
        {
            var user = await this.userService.UserManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            // get my amphora
            this.Amphorae = await amphoraeService.AmphoraStore.QueryAsync<AmphoraModel>(a => a.OrganisationId == user.OrganisationId);
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
            this.Amphorae = await amphoraeService.AmphoraStore.QueryAsync<AmphoraModel>(a => a.CreatedBy == user.Id);
            return Page();
        }
    }
}
