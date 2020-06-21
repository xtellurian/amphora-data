using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Options;
using Amphora.Identity.AspNet;
using Amphora.Identity.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Identity.Pages
{
    [SecurityHeaders]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public class LoginPageModel : LoginPageModelBase
    {
        public LoginPageModel(IIdentityServerInteractionService interaction,
                              UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager,
                              IClientStore clientStore,
                              IOptionsMonitor<ExternalServices> externalOptions,
                              IAuthenticationSchemeProvider schemeProvider,
                              ILogger<LoginPageModelBase> logger,
                              IEventRoot eventService) : base(interaction,
                                                              userManager,
                                                              signInManager,
                                                              clientStore,
                                                              externalOptions,
                                                              schemeProvider,
                                                              logger,
                                                              eventService)
        {
        }

        public async Task<IActionResult> OnGetAsync(string? returnUrl)
        {
            returnUrl ??= options.WebAppUri().ToStandardString() + "/Challenge";
            await BuildModel(returnUrl);

            if (IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return RedirectToAction("Challenge", "External", new { provider = ExternalLoginScheme, returnUrl });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl)
        {
            return await HandleLogin(returnUrl);
        }
    }
}