using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Amphorae.Pages
{
    public class NotFoundPageModel: PageModel
    {
        public IActionResult OnGet()
        {
            return Page();
        }
    }
}