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

        public class InputModel
        {
            [BindProperty]
            [Display(Name = "Latitude")]
            public double? Lat { get; set; }
            [BindProperty]
            [Display(Name = "Longitude")]
            public double? Lon { get; set; }
        }

        [BindProperty(SupportsGet = true)]
        public string Term { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public string Token { get; set; }

        public IEnumerable<AmphoraModel> Entities { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var response = await authenticateService.GetToken(User);
            if(response.success)
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
            if (Input != null && Input.Lat.HasValue && Input.Lon.HasValue)
            {
                var res = await marketService.searchService.SearchAmphora(Term, SearchParameters.GeoSearch(Input.Lat.Value, Input.Lon.Value, 10));
                this.Entities = res.Results.Select(e => e.Entity);
            }
            else
            {
                this.Entities = await marketService.FindAsync(Term);
            }

        }
    }
}