// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Options;
using Amphora.Common.Models.Platform;
using Amphora.Common.Security;
using IdentityServer4.Models;
using Newtonsoft.Json;

namespace Amphora.Identity
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
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

        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource(Common.Security.Resources.WebApp, "The Amphora Data WebApp")
            };

        public static IEnumerable<Client> Clients(ExternalServices externalServices, EnvironmentInfo envInfo, string mvcClientSecret)
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

        private const string AppUrl = "app.amphoradata.com";
        private static ICollection<string> StandardRedirectUrls(ExternalServices externalServices, EnvironmentInfo envInfo)
        {
            var urls = new List<string>();
            if (envInfo.IsDevelopment)
            {
                urls.Add("localhost:5001".ToUri().ToStandardString() + "/signin-oidc");
                urls.Add(externalServices.WebAppBaseUrl?.ToUri().ToStandardString() + "/signin-oidc");
            }
            else
            {
                if (string.IsNullOrEmpty(envInfo.Location))
                {
                    urls.Add($"{envInfo.Stack}.{AppUrl}".ToUri().ToStandardString() + "/signin-oidc");
                }
                else
                {
                    urls.Add($"{envInfo.Stack}.{envInfo.Location}.{AppUrl}".ToUri().ToStandardString() + "/signin-oidc");
                }

                urls.Add(AppUrl.ToUri().ToStandardString() + "/signin-oidc");
            }

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
            if (envInfo.IsDevelopment)
            {
                urls.Add("localhost:5001".ToUri().ToStandardString() + "/signout-callback-oidc");
                urls.Add(externalServices.WebAppBaseUrl?.ToUri().ToStandardString() + "/signout-callback-oidc");
            }
            else
            {
                urls.Add($"{envInfo.Stack}.{envInfo.Location}.{AppUrl}".ToUri().ToStandardString() + "/signout-callback-oidc");
                urls.Add(AppUrl.ToUri().ToStandardString() + "/signout-callback-oidc");
            }

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
            else if (envInfo.IsDevelopment)
            {
                logoutRedirect = "localhost:5001".ToUri().ToStandardString() + "/signout-oidc";
            }
            else if (envInfo.Stack?.ToLower() == "prod")
            {
                logoutRedirect = $"{AppUrl.ToUri().ToStandardString()}/signout-oidc";
            }
            else if (!string.IsNullOrEmpty(envInfo.Stack))
            {
                logoutRedirect = $"{envInfo.Stack}.{AppUrl}".ToUri().ToStandardString() + "/signout-callback-oidc";
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