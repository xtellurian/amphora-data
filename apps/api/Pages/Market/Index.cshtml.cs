using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Models.Search;
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
        public MarketSearch SearchDefinition { get; set; } = new MarketSearch();
        public long Count { get; set; }

        public IEnumerable<AmphoraModel> Entities { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var geo = GetGeo();
            this.Count = await marketService.CountAsync(SearchDefinition.Term,
                                                        geo,
                                                        SearchDefinition.Dist,
                                                        SearchDefinition.Skip,
                                                        SearchDefinition.Top) ?? 0;
            await RunSearch();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var geo = GetGeo();
            this.Count = await marketService.CountAsync(SearchDefinition.Term,
                                                         geo,
                                                         SearchDefinition.Dist,
                                                         SearchDefinition.Skip,
                                                         SearchDefinition.Top) ?? 0;
            await RunSearch();
            return Page();
        }

        private async Task RunSearch()
        {
            var geo = GetGeo();
            this.Entities = await marketService.FindAsync(SearchDefinition.Term,
                                                          geo,
                                                          SearchDefinition.Dist,
                                                          SearchDefinition.Skip,
                                                          SearchDefinition.Top);
        }

        private GeoLocation GetGeo()
        {
            GeoLocation geo = null;
            if (SearchDefinition.Lat.HasValue && SearchDefinition.Lon.HasValue)
            {
                geo = new GeoLocation(SearchDefinition.Lon.Value, SearchDefinition.Lat.Value);
            }
            if (SearchDefinition.Skip == null) SearchDefinition.Page = 0;
            if (SearchDefinition.Top == null) SearchDefinition.Top = 8;
            return geo;
        }
    }
}