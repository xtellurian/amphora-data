using System.Threading.Tasks;
using Amphora.Api.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Stores.Cosmos
{
    public abstract class CosmosStoreBase
    {
        protected readonly CosmosClient Client;
        protected readonly Database Database;
        protected Container Container;

        public CosmosStoreBase(IOptionsMonitor<CosmosOptions> options, string containerName, string partitionKey)
        {
            this.Client = new CosmosClient(options.CurrentValue.Endpoint, options.CurrentValue.Key);
            this.Database = Client.GetDatabase(options.CurrentValue.Database);
        }

        protected virtual async Task Init()
        {
            if (this.Container != null) return;
            var response = await this.Database.CreateContainerIfNotExistsAsync("Amphora", "/OrganisationId");
            Container = response.Container;
        }
    }
}