using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Market
{
    public class IndexModel : PageModel
    {
        private readonly IMarketService marketService;

        public IndexModel(IMarketService marketService)
        {
            this.marketService = marketService;
        }

        [BindProperty(SupportsGet = true)]
        public string Term { get; set; }

        [BindProperty]
        public bool ShowAmphorae{ get; set; }
        [BindProperty]
        public bool ShowTemporae { get; set; }

        public IEnumerable<MarketEntity> Entities { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            this.Entities = await marketService.FindAsync(Term,
                new Models.SearchParams
                {
                    IncludeAmphorae = true,
                    IncludeTemporae = true
                });
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            this.Entities = await marketService.FindAsync(Term,
                new Models.SearchParams
                {
                    IncludeAmphorae = this.ShowAmphorae,
                    IncludeTemporae = this.ShowTemporae
                });
            return Page();
        }
    }
}