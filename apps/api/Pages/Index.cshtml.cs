using Amphora.Api.AspNet;
using Amphora.Api.Options;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Pages
{
    public class IndexPageModel : PageModelBase
    {
        public IndexPageModel()
        {
        }

        public IActionResult OnGet()
        {
            return RedirectToPage("./Quickstart");
        }
    }
}