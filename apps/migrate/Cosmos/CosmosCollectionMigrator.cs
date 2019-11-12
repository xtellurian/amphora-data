using System;
using System.Threading.Tasks;
using Amphora.Migrate.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Amphora.Migrate.Cosmos
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

        public async Task MoveContainerAsync()
        {
            var clientOptions = new CosmosClientOptions() { AllowBulkExecution = false };
            var sourceClient = new CosmosClient(options.Source?.GenerateConnectionString(options.Source.PrimaryReadonlyKey), clientOptions);
            var sourceContainer = sourceClient.GetContainer(options.Source?.Database, options.Source?.Container);

            var sinkClient = new CosmosClient(options.Sink?.GenerateConnectionString(options.Sink.PrimaryKey), clientOptions);
            var sinkDatabase = sinkClient.GetDatabase(options.Sink?.Database);
            var sinkContainer = sinkDatabase.GetContainer(options.Sink?.Container);
            
            await sinkDatabase.CreateContainerIfNotExistsAsync(options.Sink?.Container, "/__partitionKey");


            var queryDefinition = new QueryDefinition("SELECT * from c");
            var iterator = sourceContainer.GetItemQueryIterator<dynamic>(queryDefinition);

            while (iterator.HasMoreResults)
            {
                var item = await iterator.ReadNextAsync();
                logger.LogInformation($"StatusCode: {item.StatusCode}");

                foreach(var i in item.Resource)
                {
                    logger.LogInformation($"Migrating Item {i.id}");
                    var res = await sinkContainer.UpsertItemAsync(i, PartitionKey.None); // FIXME: partition key null
                }

            }
            logger.LogInformation("Done Migrating");
        }
    }

    public class DeviceInformationItem
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "deviceId")]
        public string DeviceId { get; set; }

        [JsonProperty(PropertyName = "_partitionKey", NullValueHandling = NullValueHandling.Ignore)]
        public string PartitionKey { get; set; }
    }
}
