using System.Linq;
using System.Threading.Tasks;
using Amphora.Identity.IdentityConfig;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace Amphora.Identity.Stores.IdentityServer
{
    public class InMemoryClientStore : IClientStore
    {
        private readonly IIdentityServerConfig config;

        public InMemoryClientStore(IIdentityServerConfig config)
        {
            this.config = config;
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            return Task.FromResult(config.Clients().FirstOrDefault(_ => _.ClientId == clientId));
        }
    }
}