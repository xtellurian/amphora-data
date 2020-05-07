using System;
using System.Threading.Tasks;
using Amphora.Migrate.Contracts;
using Amphora.Migrate.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Amphora.Migrate.Migrators.Cosmos
{
    public class Version_0_10_0 : CosmosMigratorBase, IMigrator
    {
        public Version_0_10_0(IOptionsMonitor<CosmosMigrationOptions> options, ILogger<CosmosMigratorBase> logger)
        : base(options, logger)
        {
        }

        public async Task MigrateAsync()
        {
            var container = await GetSinkContainerAsync();

            await MoveTermsAndConditionsToGlobalTermsOfUse(container);
            await UpdateAmphoraModelTermsOfUseId(container);
            await UpdateTermsAndConditionsAcceptedToNew(container);
        }

        private async Task UpdateAmphoraModelTermsOfUseId(Container container)
        {
            var queryDefinition = new QueryDefinition("SELECT * from c where c.Discriminator = 'AmphoraModel'");
            var iterator = container.GetItemQueryIterator<dynamic>(queryDefinition);

            while (iterator.HasMoreResults)
            {
                var item = await iterator.ReadNextAsync();
                logger.LogInformation($"StatusCode: {item.StatusCode}");

                foreach (var i in item.Resource)
                {
                    logger.LogInformation($"Migrating Item {i.id}");
                    i.IsMigrated = true;
                    i.DateTimeMigrated = DateTime.Now;
                    i.TermsOfUseId = i.TermsAndConditionsId;

                    string s = JsonConvert.SerializeObject(i);

                    string idToReplace = i.id;
                    logger.LogInformation($"Replacing Item {idToReplace}");
                    var res = await container.ReplaceItemAsync(i, idToReplace, PartitionKey.None);
                }
            }
        }

        private async Task MoveTermsAndConditionsToGlobalTermsOfUse(Container container)
        {
            var queryDefinition = new QueryDefinition("SELECT * from c where c.Discriminator = 'OrganisationModel' and ARRAY_LENGTH(c.TermsAndConditions) > 0");
            var iterator = container.GetItemQueryIterator<dynamic>(queryDefinition);

            while (iterator.HasMoreResults)
            {
                var item = await iterator.ReadNextAsync();
                logger.LogInformation($"StatusCode: {item.StatusCode}");

                foreach (var i in item.Resource)
                {
                    logger.LogInformation($"Migrating Item {i.id}");
                    if (i.TermsAndConditions == null)
                    {
                        throw new NullReferenceException($"TermsAndConditions array was null for {i.id}");
                    }

                    foreach (var tnc in i.TermsAndConditions)
                    {
                        if (tnc == null)
                        {
                            logger.LogWarning("TNC is null");
                            throw new NullReferenceException("TNC was null :( ");
                        }

                        tnc.id = $"TermsOfUseModel|{tnc.Id}";
                        tnc.Discriminator = "TermsOfUseModel";
                        tnc.CreatedDate = DateTime.UtcNow;
                        tnc.LastModified = DateTime.UtcNow;
                        tnc.ttl = -1;

                        // now create that globally
                        string idToCreate = tnc.id;
                        logger.LogInformation($"Creating Item {idToCreate}");
                        var res = await container.CreateItemAsync(tnc, PartitionKey.None);
                    }
                }
            }
        }

        private async Task UpdateTermsAndConditionsAcceptedToNew(Container container)
        {
            var queryDefinition = new QueryDefinition("SELECT * from c where c.Discriminator = 'OrganisationModel' and ARRAY_LENGTH(c.TermsAndConditionsAccepted) > 0");
            var iterator = container.GetItemQueryIterator<dynamic>(queryDefinition);

            while (iterator.HasMoreResults)
            {
                var item = await iterator.ReadNextAsync();
                logger.LogInformation($"StatusCode: {item.StatusCode}");

                foreach (var i in item.Resource)
                {
                    logger.LogInformation($"Migrating Item {i.id}");

                    foreach (var tncAccepted in i.TermsAndConditionsAccepted)
                    {
                        if (tncAccepted == null)
                        {
                            logger.LogWarning("TNC Accepted was null???");
                            continue;
                        }

                        tncAccepted.Id = System.Guid.NewGuid().ToString();
                        tncAccepted.id = $"TermsOfUseAcceptedModel|{tncAccepted.Id}";
                        tncAccepted.Discriminator = "TermsOfUseAcceptedModel";
                        tncAccepted.CreatedDate = DateTime.UtcNow;
                        tncAccepted.LastModified = DateTime.UtcNow;
                        tncAccepted.ttl = -1;
                        tncAccepted.TermsOfUseId = tncAccepted.TermsAndConditionsId;
                    }

                    string idToReplace = i.id;
                    logger.LogInformation($"Replacing Item {idToReplace}");
                    var res = await container.ReplaceItemAsync(i, idToReplace, PartitionKey.None);
                }
            }
        }
    }
}