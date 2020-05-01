using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Options;
using Amphora.Common.Models.Platform;
using Amphora.Identity.Models;
using Amphora.Identity.Models.ViewModels;
using IdentityServer4.Quickstart.UI;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Identity.Pages.Account
{
    [SecurityHeaders]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public class LoginPageModel : PageModelBase
    {
        private readonly IIdentityServerInteractionService interaction;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IClientStore clientStore;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IAuthenticationSchemeProvider schemeProvider;
        private readonly ILogger<LoginPageModel> logger;
        private readonly ExternalServices options;

        public bool AllowRememberLogin { get; set; } = true;
        public bool EnableLocalLogin { get; set; } = true;

        public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProvider>();
        public IEnumerable<ExternalProvider> VisibleExternalProviders => ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

        public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders?.Count() == 1;
        public string? ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;

        public LoginPageModel(IIdentityServerInteractionService interaction,
                              UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager,
                              IClientStore clientStore,
                              IOptionsMonitor<ExternalServices> externalOptions,
                              IAuthenticationSchemeProvider schemeProvider,
                              ILogger<LoginPageModel> logger,
                              IEventRoot eventService)
        {
            this.interaction = interaction;
            this.userManager = userManager;
            this.clientStore = clientStore;
            this.signInManager = signInManager;
            this.schemeProvider = schemeProvider;
            this.logger = logger;
            EventService = eventService;
            this.options = externalOptions.CurrentValue;
        }

        [BindProperty]
        public LoginRequest Input { get; set; } = new LoginRequest();

        public string? ReturnUrl { get; private set; }
        public IEventRoot EventService { get; }

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

        public async Task<IActionResult> OnPostLoginAsync(string? returnUrl)
        {
            returnUrl ??= options.WebAppUri().ToStandardString() + "/Challenge";
            await BuildModel(returnUrl);
            // check if we are in the context of an authorization request
            var context = await interaction.GetAuthorizationContextAsync(this.ReturnUrl);

            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(Input.Username, Input.Password, false, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var user = await userManager.FindByNameAsync(Input.Username);
                    // await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.ClientId));

                    if (context != null)
                    {
                        if (await clientStore.IsPkceClientAsync(context.ClientId))
                        {
                            // if the client is PKCE then we assume it's native, so this change in how to
                            // return the response is for better UX for the end user.
                            return this.LoadingPage("/Redirect", this.ReturnUrl ?? "~");
                        }

                        // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return Redirect(this.ReturnUrl);
                    }

                    // request for a local page
                    if (Url.IsLocalUrl(returnUrl) || returnUrl?.StartsWith(options.WebAppUri().ToString()) == true)
                    {
                        return Redirect(returnUrl);
                    }
                    else if (string.IsNullOrEmpty(this.ReturnUrl))
                    {
                        return Redirect("~/");
                    }
                    else
                    {
                        // user might have clicked on a malicious link - should be logged
                        logger.LogError($"Invalid Return Url: {this.ReturnUrl}");
                        ModelState.AddModelError(string.Empty, $"Failed to redirect. The redirect URL {this.ReturnUrl} was invalid.");
                        return Page();
                    }
                }

                // await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials", clientId: context?.ClientId));
                ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
                return Page();
            }
            else
            {
                return Page();
            }
        }

        private async Task BuildModel(string returnUrl)
        {
            this.ReturnUrl = returnUrl;
            var context = await interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null && await schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == IdentityServer4.IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                this.EnableLocalLogin = local;
                this.Input.Username = context?.LoginHint ?? this.Input.Username;

                if (!local && context?.IdP != null)
                {
                    this.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
                }

                return;
            }

            var schemes = await schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null ||
                                x.Name.Equals(AccountOptions.WindowsAuthenticationSchemeName, StringComparison.OrdinalIgnoreCase))
                            .Select(x => new ExternalProvider
                            {
                                DisplayName = x.DisplayName ?? x.Name,
                                AuthenticationScheme = x.Name
                            }).ToList();

            var allowLocal = true;
            if (context?.ClientId != null)
            {
                var client = await clientStore.FindEnabledClientByIdAsync(context.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            AllowRememberLogin = AccountOptions.AllowRememberLogin;
            EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin;
            Input.Username = context?.LoginHint ?? Input.Username;
            ExternalProviders = providers;
        }
    }
}