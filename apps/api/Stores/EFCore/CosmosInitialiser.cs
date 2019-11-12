using System;
using System.Threading.Tasks;
using Amphora.Api.DbContexts;
using Amphora.Common.Configuration.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Stores.EFCore
{
    public class CosmosInitialiser
    {
        private readonly IOptionsMonitor<CosmosOptions> cosmos;
        private readonly ILogger<CosmosInitialiser> logger;
        private readonly string containerName;

        public CosmosInitialiser(IOptionsMonitor<CosmosOptions> cosmos, ILogger<CosmosInitialiser> logger)
        {
            this.cosmos = cosmos;
            this.logger = logger;
            this.containerName = nameof(AmphoraContext); // can be change by modelBuilder.HasDefaultContainer("Store");
        }

        public async Task EnableCosmosTimeToLive()
        {
            if(cosmos.CurrentValue?.Endpoint != null && cosmos.CurrentValue?.PrimaryKey != null  && cosmos.CurrentValue?.Database != null )
            {
                logger.LogInformation($"Enabling Cosmos TTL on database {cosmos.CurrentValue?.Database} and container {containerName}");
                var ttl =  cosmos.CurrentValue?.DefaultTimeToLive ?? -1; 
                var client = new CosmosClient(cosmos.CurrentValue.Endpoint, cosmos.CurrentValue.PrimaryKey);
                var container = client.GetContainer(cosmos.CurrentValue.Database, containerName);

                var containerResponse = await container.ReadContainerAsync();
                
                ContainerProperties containerProperties = containerResponse;
                if(containerProperties.DefaultTimeToLive != ttl )
                {
                    logger.LogWarning($"Setting TTL to {ttl}");
                    containerProperties.DefaultTimeToLive = ttl; // never expire by default
                    await container.ReplaceContainerAsync(containerProperties);
                }
            }
            else
            {
                logger.LogWarning("Cosmos connection info not provided.");
            }
        }
    }
}