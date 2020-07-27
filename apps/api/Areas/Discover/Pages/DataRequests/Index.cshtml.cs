using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Models.Dtos.Search;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.DataRequests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Discover.Pages.DataRequests
{
    public class IndexPageModel : PageModel
    {
        private readonly ISearchService searchService;

        public IndexPageModel(ISearchService searchService)
        {
            this.searchService = searchService;
        }

        [BindProperty(SupportsGet = true)]
        public DataRequestSearchQueryOptions SearchDefinition { get; set; } = new DataRequestSearchQueryOptions();
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
            var parameters = SearchDefinition.ToSearchParameters();
            var res = await searchService.SearchAsync<DataRequestModel>(SearchDefinition.Term, parameters);

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

            return geo;
        }
    }
}