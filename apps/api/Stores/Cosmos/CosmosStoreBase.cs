using Amphora.Api.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Stores.Cosmos
{
    public abstract class CosmosStoreBase
    {
        protected readonly CosmosClient Client;
        protected readonly Database Database;

        public CosmosStoreBase(IOptionsMonitor<CosmosOptions> options)
        {
            this.Client = new CosmosClient(options.CurrentValue.Endpoint, options.CurrentValue.Key);
            this.Database = Client.GetDatabase(options.CurrentValue.Database);
        }
    }
}