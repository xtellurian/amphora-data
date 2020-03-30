using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Amphora.Common.Models.Dtos.Users;
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
        public UpdateAmphoraUser Input { get; set; } = new UpdateAmphoraUser();

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

            SetProperties();
            return Page();
        }

        private void SetProperties()
        {
            Input.About = AppUser?.About;
            Input.FullName = AppUser?.FullName;
            Input.PhoneNumber = AppUser?.PhoneNumber;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            AppUser = await userManager.GetUserAsync(User);

            if (ModelState.IsValid && AppUser != null)
            {
                AppUser.About = Input.About;
                AppUser.FullName = Input.FullName;
                AppUser.PhoneNumber = Input.PhoneNumber;
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