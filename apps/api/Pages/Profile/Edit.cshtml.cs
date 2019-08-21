using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Profile
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IUserManager<ApplicationUser> userManager;

        public ApplicationUser AppUser { get; set; }
        [BindProperty]
        public string FullName { get; set; }
        [BindProperty]
        public string About { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }
        public EditModel(IUserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            AppUser = await userManager.GetUserAsync(User);

            if (AppUser == null)
            {
                return RedirectToPage("./Missing");
            }
            About = AppUser.About;
            FullName = AppUser.FullName;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                AppUser = await userManager.GetUserAsync(User);
                AppUser.About = About;
                AppUser.FullName = FullName;
                
                var response = await userManager.UpdateAsync(AppUser);
                if (response.Succeeded) return RedirectToPage("./Index");
                else
                {
                    foreach (var e in response.Errors)
                    {
                        ErrorMessage += e.Description + '\n';
                    }
                }
                return Page();
            }
            else
            {
                ErrorMessage = "Invalid User Details";
                return Page();
            }
        }
    }
}