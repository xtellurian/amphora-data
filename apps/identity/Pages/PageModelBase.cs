using Amphora.Identity.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Identity.Pages
{
    public abstract class PageModelBase : PageModel
    {
        public IActionResult LoadingPage(string pageName, string redirectUri)
        {
            this.HttpContext.Response.StatusCode = 200;
            this.HttpContext.Response.Headers["Location"] = "";

            return RedirectToPage(pageName, new RedirectViewModel { RedirectUrl = redirectUri });
        }
    }
}