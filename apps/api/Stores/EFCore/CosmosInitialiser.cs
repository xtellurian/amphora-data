using System;
using System.Threading.Tasks;
using Amphora.Api.DbContexts;
using Amphora.Api.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Stores.EFCore
{
    public class CosmosInitialiser: IDisposable
    {
        private readonly IOptionsMonitor<CosmosOptions> cosmos;
        private readonly string containerName;

        public CosmosInitialiser(AmphoraContext context, IOptionsMonitor<CosmosOptions> cosmos)
        {
            this.cosmos = cosmos;
            this.containerName = nameof(AmphoraContext); // can be change by modelBuilder.HasDefaultContainer("Store");
        }

        public void Dispose()
        {
            
        }

        public void EnableCosmosTimeToLive()
        {
            return;
            if(cosmos.CurrentValue?.Endpoint != null && cosmos.CurrentValue?.PrimaryKey != null  && cosmos.CurrentValue?.Database != null )
            {
                var client = new CosmosClient(cosmos.CurrentValue.Endpoint, cosmos.CurrentValue.PrimaryKey);
                var container = client.GetContainer(cosmos.CurrentValue.Database, containerName);
            }
            return;
            // var x = (new ContainerProperties
            // {
            //     Id = "container",
            //     PartitionKeyPath = "/myPartitionKey",
            //     DefaultTimeToLive = -1 //(never expire by default)
            // });
        }
    }
}