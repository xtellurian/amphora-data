// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Amphora.Common.Extensions;
using IdentityServer4.Models;

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
                new IdentityResource
                {
                    Name = "organisation",
                    Description = "Amphora Organisation",
                    UserClaims = { "organisation_id" }
                }
            };

        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("api1", "My API #1")
            };

        public static IEnumerable<Client> Clients(IEnumerable<string> baseUrls, string mvcClientSecret)
        {
            ICollection<string> redirectUris = new List<string>(baseUrls.Select(s => $"{s.ToUri()}signin-oidc"));
            redirectUris.AddRange(baseUrls.Select(s => $"{s.ToUri(https: false)}signin-oidc")); // allow http redirect
            ICollection<string> postLogoutRedirects = new List<string>(baseUrls.Select(s => $"{s.ToUri()}signout-callback-oidc"));
            postLogoutRedirects.AddRange(baseUrls.Select(s => $"{s.ToUri(https: false)}signout-callback-oidc")); // allow http redirect

            ICollection<string> logoutUris = new List<string>(baseUrls.Select(s => $"{s.ToUri()}signout-oidc"));

            return new Client[]
            {
                // MVC client using code flow + pkce
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    RequireConsent = false,
                    AllowedGrantTypes = GrantTypes.ImplicitAndClientCredentials.Union(GrantTypes.ResourceOwnerPassword).ToList(),
                    RequirePkce = true,

                    ClientSecrets = { new Secret(mvcClientSecret) },

                    RedirectUris = redirectUris,
                    FrontChannelLogoutUri = logoutUris.FirstOrDefault(),
                    PostLogoutRedirectUris = postLogoutRedirects,

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "email", "api1", "organisation" }
                }
            };
        }
    }
}