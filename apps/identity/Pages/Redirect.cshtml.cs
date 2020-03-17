using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Identity.Pages
{
    public class RedirectPageModel : PageModel
    {
        public string? RedirectUrl { get; private set; }
        public IActionResult OnGet(string? redirectUrl)
        {
            this.RedirectUrl = redirectUrl;
            return Page();
        }
    }
}