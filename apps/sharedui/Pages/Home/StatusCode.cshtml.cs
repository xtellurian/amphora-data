using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.SharedUI.Pages.Home
{
    public class StatusCodeModel : PageModel
    {
        public string Code { get; private set; }

        public IActionResult OnGet(string code)
        {
            this.Code = code;
            return Page();
        }
    }
}