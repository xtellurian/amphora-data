using Amphora.Api.AspNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages
{
    [CommonAuthorize]
    public class ChallengePageModel : PageModel
    {
        public IActionResult OnGet()
        {
            return RedirectToPage("/Quickstart");
        }
    }
}