using System.Threading.Tasks;
using Amphora.Migrate.Contracts;
using Amphora.Migrate.Models;
using Amphora.Migrate.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Migrate.Migrators
{
    public class CosmosDocumentDeleteMigrator : IMigrator
    {
        private readonly string discriminator;
        private readonly CosmosMigrationOptions options;
        private readonly ILogger<CosmosDocumentDeleteMigrator> logger;

        public CosmosDocumentDeleteMigrator(IOptionsMonitor<CosmosMigrationOptions> options,
                                            ILogger<CosmosDocumentDeleteMigrator> logger)
        {
            this.discriminator = "AmphoraSignalModel";
            this.options = options.CurrentValue;
            this.logger = logger;
        }

        public async Task MigrateAsync()
        {
            var clientOptions = new CosmosClientOptions() { AllowBulkExecution = false };
            var sourceClient = new CosmosClient(options.Source?.Cosmos?.GenerateConnectionString(options.GetSource()?.PrimaryKey), clientOptions);
            var sourceContainer = sourceClient.GetContainer(options.GetSource()?.Database, options.Source?.Cosmos?.Container);
            var sourceContainerProperties = await sourceContainer.ReadContainerAsync();

            var queryDefinition = new QueryDefinition($"SELECT * from c where c.Discriminator = '{this.discriminator}'");
            var iterator = sourceContainer.GetItemQueryIterator<GenericEntity>(queryDefinition);

            while (iterator.HasMoreResults)
            {
                var item = await iterator.ReadNextAsync();
                logger.LogInformation($"StatusCode: {item.StatusCode}");
                foreach (var i in item.Resource)
                {
                    logger.LogInformation($"Migrating Item {i.Id}");
                    var res = await sourceContainer.DeleteItemAsync<GenericEntity>(i.Id, PartitionKey.None);
                    logger.LogInformation($"Deleted Entity {i.Id}, Status Code: {res.StatusCode}");
                }
            }
        }
    }
}