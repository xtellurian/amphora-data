using System.Collections.Generic;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Applications;
using Amphora.Common.Models.Options;
using Amphora.Common.Models.Platform;

namespace Amphora.Infrastructure.Stores.Applications.OurApps
{
    public static class OurAppsProduction
    {
        private static string locationId = "6b9d70ee-d499-4d77-b5e9-b352de5e2df67";
        private const string RootDomain = "amphoradata.com";
        private static string AppHost => $"app.{RootDomain}";
        private static string ApiHost => $"api.{RootDomain}";
        public static List<ApplicationModel> Get(EnvironmentInfo envInfo, ExternalServices externalServices)
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
                    Locations = new List<ApplicationLocationModel>
                    {
                        new ApplicationLocationModel()
                        {
                            Id = $"web-{locationId}-stack-loc",
                            Origin = $"{envInfo.Stack}.{envInfo.Location}.{AppHost}".ToUri().ToStandardString(),
                            AllowedRedirectPaths = new List<string> { "/signin-oidc" },
                            PostLogoutRedirects = StandardPostLogoutRedirects(envInfo, externalServices, AppHost)
                        },
                        new ApplicationLocationModel()
                        {
                            Id = $"web-{locationId}-stack",
                            Origin = $"{envInfo.Stack}.{AppHost}".ToUri().ToStandardString(),
                            AllowedRedirectPaths = new List<string> { "/signin-oidc" },
                            PostLogoutRedirects = StandardPostLogoutRedirects(envInfo, externalServices, AppHost)
                        },
                        new ApplicationLocationModel()
                        {
                            Id = $"web-{locationId}-prod",
                            Origin = $"{AppHost}".ToUri().ToStandardString(),
                            AllowedRedirectPaths = new List<string> { "/signin-oidc" },
                            PostLogoutRedirects = StandardPostLogoutRedirects(envInfo, externalServices, ApiHost)
                        }
                    }
                },
                new ApplicationModel
                {
                    Id = Common.Security.OAuthClients.SPA,
                    Name = "SPA Client",
                    AllowOffline = true,
                    RequireConsent = false,
                    Locations = new List<ApplicationLocationModel>
                    {
                        new ApplicationLocationModel()
                        {
                            Id = $"spa-{locationId}-stack-loc",
                            Origin = $"{envInfo.Stack}.{envInfo.Location}.{ApiHost}".ToUri().ToStandardString(),
                            AllowedRedirectPaths = new List<string> { "/#/callback", "/silentRenew.html" },
                            PostLogoutRedirects = new List<string> { "/signout-callback-oidc" }
                        },
                        new ApplicationLocationModel()
                        {
                            Id = $"spa-{locationId}-stack",
                            Origin = $"{envInfo.Stack}.{ApiHost}".ToUri().ToStandardString(),
                            AllowedRedirectPaths = new List<string> { "/#/callback", "/silentRenew.html" },
                            PostLogoutRedirects = new List<string> { "/signout-callback-oidc" }
                        },
                        new ApplicationLocationModel()
                        {
                            Id = $"spa-{locationId}-main",
                            Origin = $"{ApiHost}".ToUri().ToStandardString(),
                            AllowedRedirectPaths = new List<string> { "/#/callback", "/silentRenew.html" },
                            PostLogoutRedirects = StandardPostLogoutRedirects(envInfo, externalServices, ApiHost)
                        }
                    },
                    LogoutUrl = StandardLogoutUrl(envInfo, externalServices, ApiHost),
                }
            };
        }

        private static ICollection<string> StandardPostLogoutRedirects(EnvironmentInfo envInfo, ExternalServices externalServices, string host)
        {
            var urls = new List<string>();

            urls.Add($"{envInfo.Stack}.{envInfo.Location}.{host}".ToUri().ToStandardString() + "/signout-callback-oidc");
            urls.Add(host.ToUri().ToStandardString() + "/signout-callback-oidc");

            if (!string.IsNullOrEmpty(envInfo.Stack))
            {
                urls.Add($"{envInfo.Stack}.{host}".ToUri().ToStandardString() + "/signout-callback-oidc");
            }

            return urls;
        }

        private static string StandardLogoutUrl(EnvironmentInfo envInfo, ExternalServices externalServices, string host)
        {
            string logoutRedirect;

            if (!string.IsNullOrEmpty(externalServices.WebAppBaseUrl))
            {
                logoutRedirect = externalServices.WebAppUri().ToStandardString();
            }
            else if (envInfo.Stack?.ToLower() == "prod")
            {
                logoutRedirect = $"{host.ToUri().ToStandardString()}/signout-oidc";
            }
            else if (!string.IsNullOrEmpty(envInfo.Stack))
            {
                logoutRedirect = $"{envInfo.Stack}.{host}".ToUri().ToStandardString() + "/signout-oidc";
            }
            else
            {
                var app = $"{envInfo.Stack}.{envInfo.Location}.{host}".ToUri().ToStandardString();
                logoutRedirect = $"{app}/signout-oidc";
            }

            return logoutRedirect;
        }
    }
}