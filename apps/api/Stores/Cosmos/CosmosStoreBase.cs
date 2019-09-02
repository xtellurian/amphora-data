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

        public CosmosStoreBase(IOptionsMonitor<CosmosOptions> options,
                               string containerName = "Amphora",
                               string partitionKey = "/OrganisationId") 
                               // partitionKey cannot be changed on an existing Cosmos database
                               // be very careful changing the partitionKey without changing the containerName
        {
            this.Client = new CosmosClient(options.CurrentValue.Endpoint, options.CurrentValue.Key);
            this.Database = Client.GetDatabase(options.CurrentValue.Database);
            ContainerName = containerName;
            PartitionKey = partitionKey;
        }

        public string ContainerName { get; }
        public string PartitionKey { get; }

        protected virtual async Task Init()
        {
            if (this.Container != null) return;
            var response = await this.Database.CreateContainerIfNotExistsAsync(ContainerName, PartitionKey);
            Container = response.Container;
        }
    }
}