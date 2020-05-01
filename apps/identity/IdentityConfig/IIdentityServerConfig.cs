using System.Collections.Generic;
using IdentityServer4.Models;

namespace Amphora.Identity.IdentityConfig
{
    public interface IIdentityServerConfig
    {
        IEnumerable<ApiResource> Apis();
        IEnumerable<Client> Clients();
        IEnumerable<IdentityResource> IdentityResources();
    }
}