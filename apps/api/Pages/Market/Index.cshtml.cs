using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Market
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public IndexModel()
        { }

        public IActionResult OnGet()
        {
            return RedirectToPage("/Index", new { area = "Discover" });
        }
    }
}