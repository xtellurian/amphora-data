using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.EntityFramework;
using Amphora.Api.Models.AzureSearch;
using Amphora.Api.Options;
using Amphora.Common.Configuration.Options;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Services.Azure
{
    // should be an singleton that ensure's that an index is built only once, but can block until ready
    public class UnifiedSearchInitialiser : SearchInitialiserBase, IAzureSearchInitialiser
    {
        public UnifiedSearchInitialiser(
            ILogger<UnifiedSearchInitialiser> logger,
            IOptionsMonitor<AzureSearchOptions> options,
            IOptionsMonitor<CosmosOptions> cosmosOptions) : base(logger, options, cosmosOptions)
        { }

        protected override string IndexerName => $"{ApiVersion.CurrentVersion.ToSemver('-')}-indexer";
        private readonly object initialiseLock = new object();

        public override async Task CreateIndexAsync()
        {
            if (isInitialised) { return; }
            var startTime = System.DateTime.Now;
            if (await serviceClient.Indexes.ExistsAsync(UnifiedSearchIndex.IndexName))
            {
                logger.LogInformation($"{UnifiedSearchIndex.IndexName} already exists. Waiting 2 seconds");
                // might still be creating, wait 5 seconds
                await Task.Delay(1000 * 2);
                isInitialised = true;
                return;
            }

            await WaitWhileCreating();

            OnStartInitialising();
            logger.LogWarning("Recreating the Unified index");
            await TryNTimes(async () => await CreateUnifiedSearch());

            var timeTaken = System.DateTime.Now - startTime;
            OnFinishInitialising();
            logger.LogInformation($"{nameof(CreateIndexAsync)} took {timeTaken.TotalSeconds}");
        }

        private async Task CreateUnifiedSearch()
        {
            var index = await TryCreateIndex();
            var dataSource = await TryCreateDatasource();
            var indexer = await TryCreateIndexer(dataSource, index);
        }

        private async Task<Index> TryCreateIndex()
        {
            Index index = new UnifiedSearchIndex();
            index.Validate();

            if (!await serviceClient.Indexes.ExistsAsync(index.Name))
            {
                index = await serviceClient.Indexes.CreateOrUpdateAsync(index);
            }

            return index;
        }

        private async Task<DataSource> TryCreateDatasource()
        {
            // these are the only models available in the search index;
            // var discriminators = $"(c.Discriminator = '{nameof(AmphoraModel)}' "
            // + $"or c.Discriminator='{nameof(OrganisationModel)}' "
            // + $"or c.Discriminator = '{nameof(DataRequestModel)}' )";
            var query = "SELECT * FROM c WHERE c._ts > @HighWaterMark ORDER BY c._ts";
            var cosmosDbConnectionString = cosmosOptions.CurrentValue.GenerateConnectionString(cosmosOptions.CurrentValue.PrimaryReadonlyKey);
            var deletionPolicy = new SoftDeleteColumnDeletionDetectionPolicy(nameof(EntityBase.IsDeleted), "true");
            var dataSource = DataSource.CosmosDb("cosmos-unified",
                                                 cosmosDbConnectionString,
                                                 nameof(AmphoraContext),
                                                 query,
                                                 true,
                                                 deletionPolicy,
                                                 $"Created by C# code {nameof(UnifiedSearchInitialiser)}");
            dataSource.Validate();
            dataSource = await serviceClient.DataSources.CreateOrUpdateAsync(dataSource);
            return dataSource;
        }

        private async Task<Indexer> TryCreateIndexer(DataSource dataSource, Index index)
        {
            var indexer = new Indexer(IndexerName, dataSource.Name, index.Name)
            {
                Schedule = new IndexingSchedule(System.TimeSpan.FromMinutes(60)),
                Parameters = new IndexingParameters
                {
                    Configuration = new Dictionary<string, object>
                    {
                        { "assumeOrderByHighWaterMarkColumn", true }
                    }
                },
                FieldMappings = new List<FieldMapping>
                {
                     new FieldMapping(nameof(ISearchable.Name), nameof(ISearchable.Name)),
                     new FieldMapping(nameof(ISearchable.Name), "PartialName")
                }
            };
            indexer.Validate();

            if (!await serviceClient.Indexers.ExistsAsync(indexer.Name))
            {
                indexer = await serviceClient.Indexers.CreateOrUpdateAsync(indexer);
            }

            return indexer;
        }
    }
}