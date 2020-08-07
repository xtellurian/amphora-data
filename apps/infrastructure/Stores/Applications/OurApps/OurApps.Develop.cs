using System.Collections.Generic;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Applications;

namespace Amphora.Infrastructure.Stores.Applications.OurApps
{
    public static class OurAppsDevelop
    {
        private static string locationId = "6b9d70ee-d499-4d77-b5e9-b352de5e2df6";
        private static List<string> devScopes = new List<string>
        {
            IdentityModel.OidcConstants.StandardScopes.Profile,
            IdentityModel.OidcConstants.StandardScopes.Email,
            Common.Security.Resources.WebApi,
            Common.Security.Scopes.AmphoraScope,
            Common.Security.Scopes.PurchaseScope
        };

        public static List<ApplicationModel> Get()
        {
            return new List<ApplicationModel>
            {
                new ApplicationModel
                {
                    Id = Common.Security.OAuthClients.WebApp,
                    Name = "Traditional Amphora Web Client",
                    AllowOffline = true,
                    RequireConsent = false,
                    AllowedGrantTypes = new List<string> { "implicit", "password", "client_credentials" },
                    AllowedScopes = devScopes,
                    Locations = new List<ApplicationLocationModel>
                    {
                        new ApplicationLocationModel
                        {
                            Id = $"web-{locationId}-localhost",
                            Origin = "https://localhost:5001",
                            AllowedRedirectPaths = new List<string>
                            {
                                "/signin-oidc"
                            },
                            PostLogoutRedirects = StandardPostLogoutRedirects("localhost:5001", true)
                        }
                    }
                },
                new ApplicationModel
                {
                    Id = Common.Security.OAuthClients.SPA,
                    Name = "SPA Client",
                    AllowOffline = true,
                    RequireConsent = false,
                    AllowedScopes = devScopes,
                    Locations = new List<ApplicationLocationModel>
                    {
                        new ApplicationLocationModel
                        {
                            Id = $"spa-{locationId}-localhost",
                            Origin = "https://localhost:5001",
                            AllowedRedirectPaths = new List<string>
                            {
                                "/#/callback", "/silentRenew.html"
                            },
                            PostLogoutRedirects = StandardPostLogoutRedirects("localhost:5001", true)
                        }
                    },
                    LogoutUrl = StandardLogoutUrl("localhost:5001", true),
                },
                new ApplicationModel
                {
                    Id = Common.Security.OAuthClients.ReactAmphoraExample,
                    Name = "React Amphora",
                    AllowOffline = false,
                    RequireConsent = true,
                    AllowedScopes = devScopes,
                    Locations = new List<ApplicationLocationModel>
                    {
                        new ApplicationLocationModel
                        {
                            Id = $"react-amphora-{locationId}-localhost",
                            Origin = "http://localhost:3000",
                            AllowedRedirectPaths = new List<string>
                            {
                                "/#/callback", "/silentRenew.html"
                            },
                            PostLogoutRedirects = StandardPostLogoutRedirects("localhost:3001", true)
                        }
                    },
                    LogoutUrl = StandardLogoutUrl("localhost:3000", false),
                }
            };
        }

        private static ICollection<string> StandardRedirectUrls(string host, bool isHttps, params string[] callbackPaths)
        {
            var urls = new List<string>();
            if (callbackPaths == null)
            {
                throw new System.ArgumentNullException($"{nameof(callbackPaths)} must not be null.");
            }

            foreach (var p in callbackPaths)
            {
                urls.Add(host.ToUri(isHttps).ToStandardString() + p);
            }

            return urls;
        }

        private static ICollection<string> StandardPostLogoutRedirects(string host, bool isHttps)
        {
            var urls = new List<string>();
            urls.Add(host.ToUri(isHttps).ToStandardString() + "/signout-callback-oidc");
            return urls;
        }

        private static string StandardLogoutUrl(string host, bool isHttps)
        {
            var logoutRedirect = host.ToUri(isHttps).ToStandardString() + "/signout-oidc";
            return logoutRedirect;
        }
    }
}