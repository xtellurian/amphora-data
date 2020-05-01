using System.Collections.Generic;
using Amphora.Common.Security;
using IdentityServer4.Models;

namespace Amphora.Identity.IdentityConfig
{
    internal abstract class ConfigBase
    {
        public virtual IEnumerable<ApiResource> Apis()
        {
            return new ApiResource[]
            {
                new ApiResource(Common.Security.Resources.WebApp, "The Amphora Data WebApp")
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
    }
}