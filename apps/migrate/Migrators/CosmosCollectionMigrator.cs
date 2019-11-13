using System;
using System.Threading.Tasks;
using Amphora.Migrate.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Amphora.Migrate.Migrators
{
    public class CosmosCollectionMigrator
    {
        private readonly CosmosMigrationOptions options;
        private readonly ILogger<CosmosCollectionMigrator> logger;

        public CosmosCollectionMigrator(IOptionsMonitor<CosmosMigrationOptions> options, ILogger<CosmosCollectionMigrator> logger)
        {
            this.options = options.CurrentValue;
            this.logger = logger;
        }

        public async Task MigrateAsync()
        {
            var clientOptions = new CosmosClientOptions() { AllowBulkExecution = false };
            var sourceClient = new CosmosClient(options.Source?.Cosmos?.GenerateConnectionString(options.GetSource()?.PrimaryReadonlyKey), clientOptions);
            var sourceContainer = sourceClient.GetContainer(options.GetSource()?.Database, options.Source?.Cosmos?.Container);
            var sourceContainerProperties = await sourceContainer.ReadContainerAsync();
            
            var sinkClient = new CosmosClient(options.Sink?.Cosmos?.GenerateConnectionString(options.GetSink()?.PrimaryKey), clientOptions);
            var sinkDatabase = sinkClient.GetDatabase(options.GetSink()?.Database);
            var sinkContainer = sinkDatabase.GetContainer(options.GetSink()?.Container);
            
            // create with same partition key path
            await sinkDatabase.CreateContainerIfNotExistsAsync(options.GetSink()?.Container, sourceContainerProperties.Resource.PartitionKeyPath);


            var queryDefinition = new QueryDefinition("SELECT * from c");
            var iterator = sourceContainer.GetItemQueryIterator<dynamic>(queryDefinition);

            while (iterator.HasMoreResults)
            {
                var item = await iterator.ReadNextAsync();
                logger.LogInformation($"StatusCode: {item.StatusCode}");

                foreach(var i in item.Resource)
                {
                    logger.LogInformation($"Migrating Item {i.id}");
                    if(options.Upsert)
                    {
                        var res = await sinkContainer.UpsertItemAsync(i); // FIXME: partition key null
                    }
                    else
                    {
                        var res = await sinkContainer.CreateItemAsync(i);
                    }
                }
            }
            logger.LogInformation("Done Migrating");
        }
    }
}
