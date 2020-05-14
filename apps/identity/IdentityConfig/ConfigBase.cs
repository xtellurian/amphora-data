using System.Collections.Generic;
using System.Linq;
using Amphora.Common.Security;
using IdentityServer4.Models;

namespace Amphora.Identity.IdentityConfig
{
    internal abstract class ConfigBase
    {
        private readonly string mvcClientSecret;

        public ConfigBase(string mvcClientSecret)
        {
            this.mvcClientSecret = mvcClientSecret;
        }

        protected abstract ICollection<string> StandardRedirectUrls(params string[] callbackPaths);
        protected abstract ICollection<string> StandardPostLogoutRedirects();
        protected abstract string StandardLogoutUrl();
        public virtual IEnumerable<ApiResource> Apis()
        {
            return new ApiResource[]
            {
                new ApiResource(Common.Security.Resources.WebApp, "The Amphora Data WebApp"),
                new ApiResource(Common.Security.Resources.WebApi, "The Amphora Data API")
            };
        }

        public virtual IEnumerable<IdentityResource> IdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource(Scopes.AmphoraScope,
                    new List<string>
                    {
                        Claims.About,
                        Claims.Email,
                        Claims.EmailConfirmed,
                        Claims.FullName,
                        Claims.GlobalAdmin
                    })
            };
        }

        public IEnumerable<Client> Clients()
        {
            return new Client[]
            {
                // MVC client using code flow + pkce
                new Client
                {
                    ClientId = OAuthClients.WebApp,
                    ClientName = "MVC Client",
                    RequireConsent = false,
                    AllowedGrantTypes = GrantTypes.ImplicitAndClientCredentials.Union(GrantTypes.ResourceOwnerPassword).ToList(),
                    RequirePkce = true,

                    ClientSecrets = { new Secret(mvcClientSecret) },

                    RedirectUris = StandardRedirectUrls("/signin-oidc"),
                    FrontChannelLogoutUri = StandardLogoutUrl(),
                    PostLogoutRedirectUris = StandardPostLogoutRedirects(),

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "email", Common.Security.Resources.WebApp, Scopes.AmphoraScope }
                },
                 // SPA client.
                new Client
                {
                    ClientId = OAuthClients.SPA,
                    ClientName = "SPA Client",
                    AllowAccessTokensViaBrowser = true,
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowOfflineAccess = true,
                    RequireConsent = false,
                    RequirePkce = true,

                    ClientSecrets = { new Secret(mvcClientSecret) },

                    RedirectUris = StandardRedirectUrls("/#/callback",  "/silentRenew.html"),
                    FrontChannelLogoutUri = StandardLogoutUrl(),
                    PostLogoutRedirectUris = StandardPostLogoutRedirects(),

                    AllowedScopes = { "openid", "profile", "email", Common.Security.Resources.WebApi, Scopes.AmphoraScope }
                }
            };
        }
    }
}