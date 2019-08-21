using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Profile
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUserManager<ApplicationUser> userManager;

        public IndexModel(IUserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [BindProperty]
        public ApplicationUser AppUser { get; set; }

        public async Task<IActionResult> OnGetAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                this.AppUser = await userManager.GetUserAsync(User);
            }
            else
            {
                AppUser = await userManager.FindByNameAsync(userName);
            }
            if (AppUser == null)
            {
                return RedirectToPage("./Missing");
            }
            return Page();
        }

    }
}