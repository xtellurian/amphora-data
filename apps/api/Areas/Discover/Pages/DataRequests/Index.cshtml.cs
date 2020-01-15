using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.DataRequests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Discover.Pages.DataRequests
{
    public class IndexPageModel : PageModel
    {
        private readonly IDiscoverService<DataRequestModel> discoverService;

        public IndexPageModel(IDiscoverService<DataRequestModel> discoverService)
        {
            this.discoverService = discoverService;
        }

        [BindProperty(SupportsGet = true)]
        public MarketSearch SearchDefinition { get; set; } = new MarketSearch();
        public long Count { get; set; }
        public IEnumerable<DataRequestModel> Entities { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
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
            var geo = GetGeo();
            var res = await discoverService.FindAsync(SearchDefinition.Term,
                                                          geo,
                                                          SearchDefinition.Dist,
                                                          SearchDefinition.Skip,
                                                          SearchDefinition.Top);

            this.Count = res.Count.HasValue ? res.Count.Value : 0;
            this.Entities = res.Results.Select(_ => _.Entity);
        }

        private GeoLocation GetGeo()
        {
            GeoLocation geo = null;
            if (SearchDefinition.Lat.HasValue && SearchDefinition.Lon.HasValue)
            {
                geo = new GeoLocation(SearchDefinition.Lon.Value, SearchDefinition.Lat.Value);
            }

            if (SearchDefinition.Skip == null) { SearchDefinition.Page = 0; }
            if (SearchDefinition.Top == null) { SearchDefinition.Top = 8; }
            return geo;
        }
    }
}