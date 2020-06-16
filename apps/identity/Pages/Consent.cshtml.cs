using System.Linq;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Identity.IdentityServer4;
using Amphora.Identity.Models.ViewModels.Consent;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Amphora.Identity.Pages
{
    [Authorize]
    public class ConsentPageModel : PageModelBase
    {
        private readonly IIdentityServerInteractionService interaction;
        private readonly IClientStore clientStore;
        private readonly IResourceStore resourceStore;
        private readonly IEventRoot events;
        private readonly ILogger<ConsentPageModel> logger;

        public ConsentPageModel(IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IResourceStore resourceStore,
            IEventRoot events,
            ILogger<ConsentPageModel> logger)
        {
            this.interaction = interaction;
            this.clientStore = clientStore;
            this.resourceStore = resourceStore;
            this.events = events;
            this.logger = logger;
        }

        public string? ReturnUrl { get; private set; }
        [BindProperty]
        public ConsentViewModel ViewModel { get; set; } = new ConsentViewModel();

        public async Task<IActionResult> OnGetAsync(string returnUrl)
        {
            this.ReturnUrl = returnUrl;
            await BuildViewModel(returnUrl);

            return Page();
        }

        public async Task<IActionResult> OnPostDeclineAsync(string returnUrl)
        {
            this.ReturnUrl = returnUrl;
            await BuildViewModel(returnUrl);
            var result = await ProcessConsent(ViewModel, false);
            if (result == null)
            {
                return BadRequest();
            }

            if (result.IsRedirect)
            {
                if (await clientStore.IsPkceClientAsync(result.ClientId))
                {
                    // if the client is PKCE then we assume it's native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage("Redirect", result?.RedirectUri ?? "~");
                }

                return Redirect(result?.RedirectUri);
            }

            if (result.HasValidationError)
            {
                ModelState.AddModelError(string.Empty, result.ValidationError);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAcceptAsync(string returnUrl)
        {
            this.ReturnUrl = returnUrl;
            await BuildViewModel(returnUrl);
            var result = await ProcessConsent(ViewModel, true);
            if (result == null)
            {
                return BadRequest();
            }

            if (result.IsRedirect)
            {
                if (await clientStore.IsPkceClientAsync(result.ClientId))
                {
                    // if the client is PKCE then we assume it's native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage("Redirect", result?.RedirectUri ?? "~");
                }

                return Redirect(result?.RedirectUri);
            }

            if (result.HasValidationError)
            {
                ModelState.AddModelError(string.Empty, result.ValidationError);
            }

            return Page();
        }

        private async Task BuildViewModel(string returnUrl)
        {
            var request = await interaction.GetAuthorizationContextAsync(returnUrl);
            if (request != null)
            {
                var client = await clientStore.FindEnabledClientByIdAsync(request.ClientId);
                if (client != null)
                {
                    var resources = await resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);
                    if (resources != null && (resources.IdentityResources.Any() || resources.ApiResources.Any()))
                    {
                        // the user is requesting access to resources
                        this.ViewModel = CreateConsentViewModel(returnUrl, request, client, resources);
                    }
                    else
                    {
                        logger.LogError("No scopes matching: {0}", request.ScopesRequested.Aggregate((x, y) => x + ", " + y));
                    }
                }
                else
                {
                    logger.LogError("Invalid client id: {0}", request.ClientId);
                }
            }
            else
            {
                this.ModelState.AddModelError(string.Empty, "Oops! something went wrong");
            }
        }

        private async Task<ProcessConsentResult?> ProcessConsent(ConsentInputModel? model, bool? accept)
        {
            if (model == null)
            {
                return null;
            }

            var result = new ProcessConsentResult();

            // validate return url is still valid
            var request = await interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            if (request == null)
            {
                return result;
            }

            ConsentResponse? grantedConsent = null;

            // user clicked 'no' - send back the standard 'access_denied' response
            if (accept == false)
            {
                grantedConsent = ConsentResponse.Denied;
                // TODO: emit event here
            }

            // user clicked 'yes' - validate the data
            else if (accept == true)
            {
                // if the user consented to some scope, build the response model
                if (model.ScopesConsented != null && model.ScopesConsented.Any())
                {
                    var scopes = model.ScopesConsented;
                    if (ConsentOptions.EnableOfflineAccess == false)
                    {
                        scopes = scopes.Where(x => x != IdentityModel.OidcConstants.StandardScopes.OfflineAccess);
                    }

                    grantedConsent = new ConsentResponse
                    {
                        RememberConsent = model.RememberConsent,
                        ScopesConsented = scopes.ToArray()
                    };

                    // TODO: emit event here
                }
                else
                {
                    result.ValidationError = ConsentOptions.MustChooseOneErrorMessage;
                }
            }
            else
            {
                result.ValidationError = ConsentOptions.InvalidSelectionErrorMessage;
            }

            if (grantedConsent != null)
            {
                // communicate outcome of consent back to identityserver
                await interaction.GrantConsentAsync(request, grantedConsent);

                // indicate that's it ok to redirect back to authorization endpoint
                result.RedirectUri = model?.ReturnUrl ?? "~";
                result.ClientId = request.ClientId;
            }
            else
            {
                // we need to redisplay the consent UI
                await BuildViewModel(ReturnUrl ?? "~");
            }

            return result;
        }

        private ConsentViewModel CreateConsentViewModel(string returnUrl,
            AuthorizationRequest request,
            Client client, Resources resources)
        {
            var vm = new ConsentViewModel
            {
                RememberConsent = ViewModel?.RememberConsent ?? true,
                ScopesConsented = ViewModel?.ScopesConsented ?? Enumerable.Empty<string>(),

                ReturnUrl = returnUrl,

                ClientName = client.ClientName ?? client.ClientId,
                ClientUrl = client.ClientUri,
                ClientLogoUrl = client.LogoUri,
                AllowRememberConsent = client.AllowRememberConsent
            };

            vm.IdentityScopes = resources.IdentityResources.Select(x => CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || ViewModel == null)).ToArray();
            vm.ResourceScopes = resources.ApiResources.SelectMany(x => x.Scopes).Select(x => CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || ViewModel == null)).ToArray();
            if (ConsentOptions.EnableOfflineAccess && resources.OfflineAccess)
            {
                vm.ResourceScopes = vm.ResourceScopes.Union(new ScopeViewModel[]
                {
                    GetOfflineAccessScope(vm.ScopesConsented.Contains(IdentityModel.OidcConstants.StandardScopes.OfflineAccess) || ViewModel == null)
                });
            }

            return vm;
        }

        private ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check)
        {
            return new ScopeViewModel
            {
                Name = identity.Name,
                DisplayName = identity.DisplayName,
                Description = identity.Description,
                Emphasize = identity.Emphasize,
                Required = identity.Required,
                Checked = check || identity.Required
            };
        }

        public ScopeViewModel CreateScopeViewModel(Scope scope, bool check)
        {
            return new ScopeViewModel
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Description = scope.Description,
                Emphasize = scope.Emphasize,
                Required = scope.Required,
                Checked = check || scope.Required
            };
        }

        private ScopeViewModel GetOfflineAccessScope(bool check)
        {
            return new ScopeViewModel
            {
                Name = IdentityModel.OidcConstants.StandardScopes.OfflineAccess,
                DisplayName = ConsentOptions.OfflineAccessDisplayName,
                Description = ConsentOptions.OfflineAccessDescription,
                Emphasize = true,
                Checked = check
            };
        }
    }
}