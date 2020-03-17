using System.Threading.Tasks;
using Amphora.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Identity.Pages.Account.Manage
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;

        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public ApplicationUser? AppUser { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            this.AppUser = await userManager.GetUserAsync(User);
            return Page();
        }
    }
}