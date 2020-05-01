using System.Collections.Generic;
using System.Linq;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Options;
using Amphora.Common.Models.Platform;
using Amphora.Common.Security;
using IdentityServer4.Models;
using Newtonsoft.Json;

namespace Amphora.Identity.IdentityConfig
{
    internal class ProductionConfig : ConfigBase, IIdentityServerConfig
    {
        private const string AppUrl = "app.amphoradata.com";
        private readonly ExternalServices externalServices;
        private readonly EnvironmentInfo envInfo;
        private readonly string mvcClientSecret;
        public ProductionConfig(ExternalServices externalServices, EnvironmentInfo envInfo, string mvcClientSecret)
        {
            this.externalServices = externalServices;
            this.envInfo = envInfo;
            this.mvcClientSecret = mvcClientSecret;
        }

        public override IEnumerable<IdentityResource> IdentityResources()
        {
            return base.IdentityResources();
        }

        public override IEnumerable<ApiResource> Apis()
        {
            return base.Apis();
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

                    RedirectUris = StandardRedirectUrls(externalServices, envInfo),
                    FrontChannelLogoutUri = StandardLogoutUrl(externalServices, envInfo),
                    PostLogoutRedirectUris = StandardPostLogoutRedirects(externalServices, envInfo),

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "email", Common.Security.Resources.WebApp, Scopes.AmphoraScope }
                }
            };
        }

        private static ICollection<string> StandardRedirectUrls(ExternalServices externalServices, EnvironmentInfo envInfo)
        {
            var urls = new List<string>();

            if (string.IsNullOrEmpty(envInfo.Location))
            {
                urls.Add($"{envInfo.Stack}.{AppUrl}".ToUri().ToStandardString() + "/signin-oidc");
            }
            else
            {
                urls.Add($"{envInfo.Stack}.{envInfo.Location}.{AppUrl}".ToUri().ToStandardString() + "/signin-oidc");
            }

            urls.Add(AppUrl.ToUri().ToStandardString() + "/signin-oidc");

            if (!string.IsNullOrEmpty(envInfo.Stack))
            {
                // add something like develop.app.amphoradata.com
                urls.Add($"{envInfo.Stack}.{AppUrl}".ToUri().ToStandardString() + "/signin-oidc");
            }

            System.Console.WriteLine($"Redirects: {JsonConvert.SerializeObject(urls, Formatting.Indented)}");
            return urls;
        }

        private static ICollection<string> StandardPostLogoutRedirects(ExternalServices externalServices, EnvironmentInfo envInfo)
        {
            var urls = new List<string>();

            urls.Add($"{envInfo.Stack}.{envInfo.Location}.{AppUrl}".ToUri().ToStandardString() + "/signout-callback-oidc");
            urls.Add(AppUrl.ToUri().ToStandardString() + "/signout-callback-oidc");

            if (!string.IsNullOrEmpty(envInfo.Stack))
            {
                urls.Add($"{envInfo.Stack}.{AppUrl}".ToUri().ToStandardString() + "/signout-callback-oidc");
            }

            System.Console.WriteLine($"Post Logout Redirects: {JsonConvert.SerializeObject(urls, Formatting.Indented)}");
            return urls;
        }

        private static string StandardLogoutUrl(ExternalServices externalServices, EnvironmentInfo envInfo)
        {
            string logoutRedirect;

            if (!string.IsNullOrEmpty(externalServices.WebAppBaseUrl))
            {
                logoutRedirect = externalServices.WebAppUri().ToStandardString();
            }
            else if (envInfo.Stack?.ToLower() == "prod")
            {
                logoutRedirect = $"{AppUrl.ToUri().ToStandardString()}/signout-oidc";
            }
            else if (!string.IsNullOrEmpty(envInfo.Stack))
            {
                logoutRedirect = $"{envInfo.Stack}.{AppUrl}".ToUri().ToStandardString() + "/signout-oidc";
            }
            else
            {
                var app = $"{envInfo.Stack}.{envInfo.Location}.{AppUrl}".ToUri().ToStandardString();
                logoutRedirect = $"{app}/signout-oidc";
            }

            System.Console.WriteLine($"Logout Redirect: {logoutRedirect}");
            return logoutRedirect;
        }
    }
}