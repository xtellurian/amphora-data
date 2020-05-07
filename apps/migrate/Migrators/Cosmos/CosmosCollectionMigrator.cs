using System;
using System.Threading.Tasks;
using Amphora.Migrate.Contracts;
using Amphora.Migrate.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Migrate.Migrators
{
    /// <summary>
    /// Takes documents from one cosmos db and moves them to another.
    /// </summary>
    public class CosmosCollectionMigrator : CosmosMigratorBase, IMigrator
    {
        private readonly CosmosMigrationOptions options;

        public CosmosCollectionMigrator(IOptionsMonitor<CosmosMigrationOptions> options, ILogger<CosmosCollectionMigrator> logger)
        : base(options, logger)
        {
            this.options = options.CurrentValue;
        }

        public async Task MigrateAsync()
        {
            var sourceContainer = await GetSourceContainerAsync();
            var sinkContainer = await GetSinkContainerAsync();

            // create with same partition key path
            if (sinkDatabase != null)
            {
                await sinkDatabase.CreateContainerIfNotExistsAsync(options.GetSink()?.Container, sourceContainerProperties?.Resource.PartitionKeyPath);
            }

            var queryDefinition = new QueryDefinition("SELECT * from c");
            var iterator = sourceContainer.GetItemQueryIterator<dynamic>(queryDefinition);

            while (iterator.HasMoreResults)
            {
                var item = await iterator.ReadNextAsync();
                logger.LogInformation($"StatusCode: {item.StatusCode}");

                foreach (var i in item.Resource)
                {
                    logger.LogInformation($"Migrating Item {i.id}");
                    i.IsMigrated = true;
                    i.DateTimeMigrated = DateTime.Now;
                    if (options.Upsert == true)
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
