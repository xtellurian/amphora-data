using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Options;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Areas.Discover.Pages
{
    [Authorize]
    public class IndexPageModel : PageModel
    {
        private readonly IMarketService marketService;
        private readonly IAuthenticateService authenticateService;

        public string MapKey { get; }

        public IndexPageModel(IMarketService marketService,
                              IAuthenticateService authenticateService,
                              IOptionsMonitor<AzureMapsOptions> mapsOptions)
        {
            this.marketService = marketService;
            this.authenticateService = authenticateService;
            this.MapKey = mapsOptions.CurrentValue?.SecondaryKey;
        }

        [BindProperty(SupportsGet = true)]
        public MarketSearch SearchDefinition { get; set; } = new MarketSearch();
        public long Count { get; set; }

        public IEnumerable<AmphoraModel> Entities { get; set; }
        public IList<FacetResult> LabelFacets { get; private set; } = new List<FacetResult>();
        public bool MapView { get; private set; }

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

        public async Task<IActionResult> OnGetMapViewAsync()
        {
            this.MapView = true;
            return await OnGetAsync();
        }

        public async Task<IActionResult> OnPostMapViewAsync()
        {
            this.MapView = true;
            return await OnPostAsync();
        }

        public async Task<IActionResult> OnGetListViewAsync()
        {
            this.MapView = false;
            return await OnGetAsync();
        }

        public async Task<IActionResult> OnPostListViewAsync()
        {
            this.MapView = false;
            return await OnPostAsync();
        }

        private ParallelQuery<string> GetLabels()
        {
            var labels = this.SearchDefinition?.Labels?.Split(',').Where(_ => !string.IsNullOrWhiteSpace(_)).AsParallel();
            labels?.ForAll(_ => _.Trim()); // trim all labels
            this.SearchDefinition.Labels = labels != null ? string.Join(',', labels) : null;
            return labels;
        }

        private async Task RunSearch()
        {
            var geo = GetGeo();
            var labels = GetLabels();
            var res = await marketService.FindAsync(SearchDefinition.Term,
                                                          geo,
                                                          SearchDefinition.Dist,
                                                          SearchDefinition.Skip,
                                                          SearchDefinition.Top,
                                                          labels);

            this.Count = res.Count.HasValue ? res.Count.Value : 0;
            this.Entities = res.Results.Select(_ => _.Entity);
            if (res.Facets.TryGetValue($"{nameof(AmphoraModel.Labels)}/{nameof(Label.Name)}", out var labelFacets))
            {
                this.LabelFacets = labelFacets;
            }
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