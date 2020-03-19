using System;
using System.Threading.Tasks;
using Amphora.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Identity.Pages.Account
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;

        public ApplicationUser? AppUser { get; set; }

        [BindProperty]
        public string? FullName { get; set; }
        [BindProperty]
        public string? About { get; set; }
        [TempData]
        public string? ErrorMessage { get; set; } = null;
        [TempData]
        public bool? Saved { get; set; } = null;
        public EditModel(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            AppUser = await userManager.GetUserAsync(User);

            if (AppUser == null)
            {
                return RedirectToPage("./UserMissing");
            }

            About = AppUser?.About;
            FullName = AppUser?.FullName;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                AppUser = await userManager.GetUserAsync(User);
                AppUser.About = About;
                AppUser.FullName = FullName;

                try
                {
                    var response = await userManager.UpdateAsync(AppUser);
                    Saved = true;
                    return Page();
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    return Page();
                }
            }
            else
            {
                ErrorMessage = "Invalid User Details";
                return Page();
            }
        }
    }
}