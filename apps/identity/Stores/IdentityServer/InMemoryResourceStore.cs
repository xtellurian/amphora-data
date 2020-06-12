using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Identity.IdentityConfig;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace Amphora.Identity.Stores.IdentityServer
{
    public class InMemoryResourceStore : IResourceStore
    {
        private readonly IIdentityServerConfig config;

        public InMemoryResourceStore(IIdentityServerConfig config)
        {
            this.config = config;
        }

        public Task<ApiResource> FindApiResourceAsync(string name)
        {
            return Task.FromResult(config.Apis().FirstOrDefault(_ => _.Name == name));
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            return Task.FromResult(config.Apis().Where(_ => _.Scopes.Any(s => scopeNames.Any(name => name == s.Name))));
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            return Task.FromResult(config.IdentityResources().Where(i => scopeNames.Contains(i.Name)));
        }

        public Task<Resources> GetAllResourcesAsync()
        {
            return Task.FromResult(new Resources(config.IdentityResources(), config.Apis()));
        }
    }
}