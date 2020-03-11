using System;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Profiles.Pages.Account
{
    [CommonAuthorize]
    public class EditModel : PageModel
    {
        private readonly IUserService userService;

        public ApplicationUser AppUser { get; set; }

        [BindProperty]
        public string FullName { get; set; }
        [BindProperty]
        public string About { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }
        public EditModel(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            AppUser = await userService.ReadUserModelAsync(User);

            if (AppUser == null)
            {
                return RedirectToPage("./UserMissing");
            }

            About = AppUser.About;
            FullName = AppUser.FullName;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                AppUser = await userService.ReadUserModelAsync(User);
                AppUser.About = About;
                AppUser.FullName = FullName;

                try
                {
                    var response = await userService.UpdateAsync(User, AppUser);
                    return RedirectToPage("./Detail");
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