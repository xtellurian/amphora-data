using System.Threading.Tasks;
using Amphora.Common.Models.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Areas.Profiles.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly IOptionsMonitor<ExternalServices> externalServices;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(IOptionsMonitor<ExternalServices> externalServices, ILogger<LogoutModel> logger)
        {
            this.externalServices = externalServices;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                var uri = $"{externalServices.CurrentValue.IdentityUri()}/Account/Logout";
                if (returnUrl != null)
                {
                    uri += $"?returnUrl={returnUrl}";
                }

                await HttpContext.SignOutAsync();
                _logger.LogInformation("User logged out.");
                return Redirect(uri);
            }
            else
            {
                return Page();
            }
        }
    }
}