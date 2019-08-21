using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Market
{
    [Authorize]
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

        public IEnumerable<Amphora.Common.Models.Amphora> Entities { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            this.Entities = await marketService.FindAsync(Term,
                new Models.SearchParams());
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            this.Entities = await marketService.FindAsync(Term,
                new Models.SearchParams());
            return Page();
        }
    }
}