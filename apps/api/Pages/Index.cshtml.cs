using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ISignInManager<ApplicationUser> signInManager;

        public IndexModel(ISignInManager<ApplicationUser> signInManager)
        {
            this.signInManager = signInManager;
        }
        public IActionResult OnGet()
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Market/Index");
            }
            else
            {
                return RedirectToPage("/Account/Login", new { area = "Identity"});
            }
        }
    }
}