using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Identity.Pages
{
    [Authorize]
    public class ChallengePageModel : PageModelBase
    {
        public IActionResult OnGet()
        {
            return RedirectToPage("./Index");
        }
    }
}