using System.Threading.Tasks;
using Amphora.Common.Models.Users;
using Amphora.Migrate.Contracts;
using Amphora.Migrate.Models;
using Amphora.Migrate.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Migrate.Migrators
{
    public class CosmosUserDataModelMigrator : CosmosMigratorBase, IMigrator
    {
        private readonly string discriminator;
        private readonly CosmosMigrationOptions options;

        public CosmosUserDataModelMigrator(IOptionsMonitor<CosmosMigrationOptions> options,
                                            ILogger<CosmosUserDataModelMigrator> logger) : base(options, logger)
        {
            this.discriminator = "ApplicationUser";
            this.options = options.CurrentValue;
        }

        public async Task MigrateAsync()
        {
            var sourceContainer = await GetSourceContainerAsync();
            var sinkContainer = await GetSinkContainerAsync();

            var queryDefinition = new QueryDefinition($"SELECT * from c where c.Discriminator = '{this.discriminator}'");
            var iterator = sourceContainer.GetItemQueryIterator<dynamic>(queryDefinition);

            while (iterator.HasMoreResults)
            {
                var item = await iterator.ReadNextAsync();
                logger.LogInformation($"StatusCode: {item.StatusCode}");
                // create the users in the new Identity Collection
                foreach (var i in item.Resource)
                {
                    logger.LogInformation($"Migrating Item {i.id}");

                    // updating the existing docs for User Data
                    i.Discriminator = nameof(ApplicationUserDataModel);
                    i.id = $"{nameof(ApplicationUserDataModel)}|{i.Id}";
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
        }
    }
}