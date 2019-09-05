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
        private readonly IUserManager userManager;

        public IndexModel(IUserManager userManager)
        {
            this.userManager = userManager;
        }

        [BindProperty]
        public IApplicationUser AppUser { get; set; }
        public bool IsSelf {get; set; }

        public async Task<IActionResult> OnGetAsync(string userName)
        {
            var user = await userManager.GetUserAsync(User);
            
            if( string.IsNullOrEmpty(userName))
            {
                this.AppUser = user;
                IsSelf = true;
            }
            else
            {
                var lookupUser = await userManager.FindByNameAsync(userName);
                IsSelf = lookupUser.Id == user.Id;
            }
            if (AppUser == null)
            {
                return RedirectToPage("./Missing");
            }

            return Page();
        }

    }
}