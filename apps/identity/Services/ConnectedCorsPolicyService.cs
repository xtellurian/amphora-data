using System.Linq;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Applications;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;

namespace Amphora.Identity.Services
{
    public class ConnectedCorsPolicyService : ICorsPolicyService
    {
        private readonly IEntityStore<ApplicationLocationModel> locationsStore;
        private readonly ILogger<ConnectedCorsPolicyService> logger;

        public ConnectedCorsPolicyService(IEntityStore<ApplicationLocationModel> locationsStore, ILogger<ConnectedCorsPolicyService> logger)
        {
            this.locationsStore = locationsStore;
            this.logger = logger;
        }

        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            // dont use Query(), use QueryAsync()
            logger.LogDebug($"Testing whether origin {origin} is allowed");
            var allApps = await locationsStore.QueryAsync(_ => _.Origin == origin);
            var count = allApps.Count();
            logger.LogInformation($"Found {count} origins matching {origin}");
            return count > 0;
        }
    }
}