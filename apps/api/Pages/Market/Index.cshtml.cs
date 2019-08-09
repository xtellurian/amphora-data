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

        public string Term { get; set; }
        public bool ShowAmphorae { get; set; }
        public bool ShowTemporae { get; set; }

        public IEnumerable<MarketEntity> Entities { get; set; }

        public async Task<IActionResult> OnGetAsync(string term, bool amphorae = true, bool temporae = true)
        {
            this.Term = term;
            this.Entities = await marketService.FindAsync(term);
            return Page();
        }
    }
}