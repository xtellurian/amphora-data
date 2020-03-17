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
        private readonly IUserDataService userDataService;

        public ApplicationUserDataModel AppUser { get; set; }

        [BindProperty]
        public string FullName { get; set; }
        [BindProperty]
        public string About { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }
        public EditModel(IUserDataService userDataService)
        {
            this.userDataService = userDataService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userReadRes = await userDataService.ReadAsync(User);
            if (!userReadRes.Succeeded)
            {
                return RedirectToPage("./UserMissing");
            }

            About = AppUser.About;
            FullName = AppUser.ContactInformation.FullName;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var userReadRes = await userDataService.ReadAsync(User);
                if (!userReadRes.Succeeded)
                {
                    return RedirectToPage("./UserMissing");
                }

                AppUser.About = About;
                AppUser.ContactInformation.FullName = FullName;

                try
                {
                    var response = await userDataService.UpdateAsync(User, AppUser);
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