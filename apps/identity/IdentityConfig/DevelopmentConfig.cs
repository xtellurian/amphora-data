using System.Collections.Generic;
using System.Linq;
using Amphora.Common.Extensions;
using Amphora.Common.Security;
using IdentityServer4.Models;
using Newtonsoft.Json;

namespace Amphora.Identity.IdentityConfig
{
    internal class DevelopmentConfig : ConfigBase, IIdentityServerConfig
    {
        private readonly string mvcClientSecret;
        public DevelopmentConfig(string mvcClientSecret)
        {
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

                    RedirectUris = StandardRedirectUrls(),
                    FrontChannelLogoutUri = StandardLogoutUrl(),
                    PostLogoutRedirectUris = StandardPostLogoutRedirects(),

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "email", Common.Security.Resources.WebApp, Scopes.AmphoraScope }
                }
            };
        }

        private static ICollection<string> StandardRedirectUrls()
        {
            var urls = new List<string>();
            urls.Add("localhost:5001".ToUri().ToStandardString() + "/signin-oidc");

            System.Console.WriteLine($"Redirects: {JsonConvert.SerializeObject(urls, Formatting.Indented)}");
            return urls;
        }

        private static ICollection<string> StandardPostLogoutRedirects()
        {
            var urls = new List<string>();
            urls.Add("localhost:5001".ToUri().ToStandardString() + "/signout-callback-oidc");

            System.Console.WriteLine($"Post Logout Redirects: {JsonConvert.SerializeObject(urls, Formatting.Indented)}");
            return urls;
        }

        private static string StandardLogoutUrl()
        {
            var logoutRedirect = "localhost:5001".ToUri().ToStandardString() + "/signout-oidc";
            System.Console.WriteLine($"Logout Redirect: {logoutRedirect}");
            return logoutRedirect;
        }
    }
}