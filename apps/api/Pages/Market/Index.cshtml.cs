using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
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
        [BindProperty]
        public int? Skip { get; set; }
        [BindProperty]
        public int? Top { get; set; }
        public long Count { get; set; }

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

        public async Task<IActionResult> OnGetAsync(int? skip, int? top)
        {
            this.Skip = skip;
            this.Top = top;
            var geo = GetGeo();
            this.Count = await marketService.CountAsync(Term, geo, Dist, Skip, Top) ?? 0;
            await RunSearch();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? skip, int? top)
        {
            this.Skip = skip;
            this.Top = top;
            await RunSearch();
            return Page();
        }

        private async Task RunSearch()
        {
            var geo = GetGeo();
            this.Entities = await marketService.FindAsync(Term, geo, Dist, Skip, Top);
        }

        private GeoLocation GetGeo()
        {
            GeoLocation geo = null;
            if (Lat.HasValue && Lon.HasValue)
            {
                geo = new GeoLocation(Lon.Value, Lat.Value);
            }
            if (Skip == null) Skip = 0;
            if (Top == null) Top = 10;
            return geo;
        }
    }
}