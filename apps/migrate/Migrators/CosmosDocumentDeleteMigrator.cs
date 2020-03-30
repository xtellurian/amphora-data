using System.Threading.Tasks;
using Amphora.Migrate.Contracts;
using Amphora.Migrate.Models;
using Amphora.Migrate.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Migrate.Migrators
{
    public class CosmosDocumentDeleteMigrator : CosmosMigratorBase, IMigrator
    {
        private readonly string discriminator;
        private readonly CosmosMigrationOptions options;
        private readonly ILogger<CosmosDocumentDeleteMigrator> logger;

        public CosmosDocumentDeleteMigrator(IOptionsMonitor<CosmosMigrationOptions> options,
                                            ILogger<CosmosDocumentDeleteMigrator> logger) : base(options, logger)
        {
            this.discriminator = "ApplicationUser";
            this.options = options.CurrentValue;
            this.logger = logger;
        }

        public async Task MigrateAsync()
        {
            var sinkContainer = await GetSourceContainerAsync();

            var queryDefinition = new QueryDefinition($"SELECT * from c where c.Discriminator = '{this.discriminator}'");
            var iterator = sinkContainer.GetItemQueryIterator<GenericEntity>(queryDefinition);

            while (iterator.HasMoreResults)
            {
                var item = await iterator.ReadNextAsync();
                logger.LogInformation($"StatusCode: {item.StatusCode}");
                foreach (var i in item.Resource)
                {
                    logger.LogInformation($"Migrating Item {i.Id}");
                    var res = await sinkContainer.DeleteItemAsync<GenericEntity>(i.Id, PartitionKey.None);
                    logger.LogInformation($"Deleted Entity {i.Id}, Status Code: {res.StatusCode}");
                }
            }
        }
    }
}