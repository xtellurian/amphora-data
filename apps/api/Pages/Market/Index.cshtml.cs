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
            ParallelQuery<string> labels = GetLabels();
            this.Count = await marketService.CountAsync(SearchDefinition.Term,
                                                         geo,
                                                         SearchDefinition.Dist,
                                                         SearchDefinition.Skip,
                                                         SearchDefinition.Top,
                                                         labels) ?? 0;
            await RunSearch();
            return Page();
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
            this.Entities = await marketService.FindAsync(SearchDefinition.Term,
                                                          geo,
                                                          SearchDefinition.Dist,
                                                          SearchDefinition.Skip,
                                                          SearchDefinition.Top,
                                                          labels);
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