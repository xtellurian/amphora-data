using System;
using System.Threading.Tasks;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Areas.Profiles.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailPageModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ConfirmEmailPageModel> logger;

        public ConfirmEmailPageModel(UserManager<ApplicationUser> userManager, ILogger<ConfirmEmailPageModel> logger)
        {
            _userManager = userManager;
            this.logger = logger;
        }

        public bool Succeeded { get; private set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                logger.LogCritical("Failed to confirm email");
                foreach (var e in result.Errors)
                {
                    logger.LogError(e.Description);
                    ModelState.AddModelError(string.Empty, e.Description);
                }
            }

            this.Succeeded = result.Succeeded;
            return Page();
        }
    }
}
