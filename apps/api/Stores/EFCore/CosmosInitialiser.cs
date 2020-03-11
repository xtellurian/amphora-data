using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.EntityFramework;
using Amphora.Common.Configuration.Options;
using Amphora.Common.Models;
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

        public async Task EnsureContainerCreated()
        {
            if (IsUsingCosmos())
            {
                logger.LogInformation($"Creating Database {cosmos.CurrentValue.Database}");
                using (var cosmosClient = new CosmosClient(cosmos.CurrentValue.Endpoint, cosmos.CurrentValue.PrimaryKey))
                {
                    try
                    {
                        var db = await cosmosClient.CreateDatabaseIfNotExistsAsync(cosmos.CurrentValue.Database, 400);
                        logger.LogInformation($"Created Cosmos Database Id: {db.Database.Id}");
                        var c = await db.Database.CreateContainerIfNotExistsAsync(containerName, "/__partitionKey");
                        logger.LogInformation($"Created Cosmos Container Id: {c.Container.Id}");
                    }
                    catch (CosmosException cEx)
                    {
                        logger.LogCritical("Error creating Cosmos container", cEx);
                    }
                }
            }
            else
            {
                logger.LogWarning("Not using CosmosDB. Connection info not provided.");
            }
        }

        public async Task EnableCosmosTimeToLive()
        {
            if (IsUsingCosmos())
            {
                logger.LogInformation($"Enabling Cosmos TTL on database {cosmos.CurrentValue?.Database} and container {containerName}");
                var ttl = cosmos.CurrentValue?.DefaultTimeToLive ?? -1;
                using (var client = new CosmosClient(cosmos.CurrentValue.Endpoint, cosmos.CurrentValue.PrimaryKey))
                {
                    var container = client.GetContainer(cosmos.CurrentValue.Database, containerName);

                    var containerResponse = await container.ReadContainerAsync();

                    ContainerProperties containerProperties = containerResponse;
                    if (containerProperties.DefaultTimeToLive != ttl)
                    {
                        logger.LogWarning($"Setting TTL to {ttl}");
                        containerProperties.DefaultTimeToLive = ttl; // never expire by default
                        await container.ReplaceContainerAsync(containerProperties);
                    }
                }
            }
            else
            {
                logger.LogWarning("Not using CosmosDB. Connection info not provided.");
            }
        }

        public async Task LogInformationAsync()
        {
            if (IsUsingCosmos())
            {
                logger.LogInformation($"Logging Cosmos Information for database {cosmos.CurrentValue?.Database} and container {containerName}");
                using (var client = new CosmosClient(cosmos.CurrentValue.Endpoint, cosmos.CurrentValue.PrimaryKey))
                {
                    var container = client.GetContainer(cosmos.CurrentValue.Database, containerName);

                    var sqlQueryText = "SELECT * FROM c";
                    var queryDefinition = new QueryDefinition(sqlQueryText);
                    var queryResultSetIterator = container.GetItemQueryIterator<dynamic>(queryDefinition);

                    long total = 0;
                    int times = 0;
                    while (queryResultSetIterator.HasMoreResults)
                    {
                        var res = await queryResultSetIterator.ReadNextAsync();
                        total += res.Count;
                        times++;
                    }

                    logger.LogInformation($"There are {total} Cosmos documents, queried {times} times.");
                }
            }
            else
            {
                logger.LogWarning("Not using CosmosDB. Connection info not provided.");
            }
        }

        private bool IsUsingCosmos()
        {
            return !string.IsNullOrEmpty(cosmos.CurrentValue?.Endpoint)
                && !string.IsNullOrEmpty(cosmos.CurrentValue?.PrimaryKey)
                && !string.IsNullOrEmpty(cosmos.CurrentValue?.Database);
        }
    }
}