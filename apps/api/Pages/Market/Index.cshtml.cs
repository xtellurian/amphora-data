using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Search;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Market
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IMarketService marketService;
        private readonly IAuthenticateService authenticateService;

        public IndexModel(IMarketService marketService, IAuthenticateService authenticateService)
        {
            this.marketService = marketService;
            this.authenticateService = authenticateService;
        }

        [BindProperty(SupportsGet = true)]
        [Display(Name = "Latitude")]
        public double? Lat { get; set; }
        [BindProperty(SupportsGet = true)]
        [Display(Name = "Longitude")]
        public double? Lon { get; set; }
        [Display(Name = "Distance")]
        public double? Dist { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Term { get; set; }

        [BindProperty(SupportsGet = true)]

        public string Token { get; set; }

        public IEnumerable<AmphoraModel> Entities { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var response = await authenticateService.GetToken(User);
            if (response.success)
            {
                Token = response.token;
            }

            await RunSearch();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await RunSearch();
            return Page();
        }

        private async Task RunSearch()
        {
            if (Lat.HasValue && Lon.HasValue)
            {
                var d = Dist.HasValue ? Dist.Value : 100; // default to 100km
                var res = await marketService.searchService.SearchAmphora(Term, SearchParameters.GeoSearch(Lat.Value, Lon.Value, d));
                this.Entities = res.Results.Select(e => e.Entity);
            }
            else
            {
                this.Entities = await marketService.FindAsync(Term);
            }
        }
    }
}