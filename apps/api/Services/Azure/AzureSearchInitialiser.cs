using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.DbContexts;
using Amphora.Api.Models.AzureSearch;
using Amphora.Api.Options;
using Amphora.Common.Configuration.Options;
using Amphora.Common.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Services.Azure
{
    // should be an singleton that ensure's that an index is built only once, but can block until ready
    public class AzureSearchInitialiser : IAzureSearchInitialiser
    {
        public AzureSearchInitialiser(
            ILogger<AzureSearchInitialiser> logger,
            IOptionsMonitor<AzureSearchOptions> options,
            IOptionsMonitor<CosmosOptions> cosmosOptions)
        {
            this.logger = logger;
            this.cosmosOptions = cosmosOptions;
            this.serviceClient = new SearchServiceClient(options.CurrentValue.Name, new SearchCredentials(options.CurrentValue.PrimaryKey));
        }
        private bool isInitialised;
        private bool isCreatingIndex;
        private readonly ILogger<AzureSearchInitialiser> logger;
        private readonly IOptionsMonitor<CosmosOptions> cosmosOptions;
        private SearchServiceClient serviceClient;
        public string IndexerName => $"{ApiVersion.CurrentVersion.ToSemver('-')}-amphora-indexer";
        private readonly object initialiseLock = new object();
        public async Task<bool> TryIndex()
        {
            // ensure created:
            if(! await serviceClient.Indexers.ExistsAsync(IndexerName))
            {
                logger.LogInformation($"Index request resulted in creation of full index.");
                await CreateAmphoraIndexAsync();
            }
            try
            {
                await serviceClient.Indexers.RunAsync(IndexerName);
                return true;
            }
            catch(System.Exception ex)
            {
                logger.LogError($"TryIndex failed for indexer {IndexerName}", ex);
                return false;
            }
        }
        public async Task CreateAmphoraIndexAsync()
        {
            if (isInitialised) return;
            var startTime = System.DateTime.Now;
            if (await serviceClient.Indexes.ExistsAsync(AmphoraSearchIndex.IndexName))
            {
                logger.LogInformation($"{AmphoraSearchIndex.IndexName} already exists. Waiting 2 seconds");
                // might still be creating, wait 5 seconds
                await Task.Delay(1000 * 2);
                isInitialised = true;
                return;
            }

            while (isCreatingIndex)
            {
                // we need to wait for it to be set to false, then return
                logger.LogInformation($"Waiting for {AmphoraSearchIndex.IndexName} to be created");
                await Task.Delay(1000);
            }

            isCreatingIndex = true;
            logger.LogWarning("Recreating the Amphora index");
            var maxAttempts = 5;
            var attempts = 0;
            while (true)
            {
                try
                {
                    var index = await TryCreateIndex();
                    var dataSource = await TryCreateDatasource();
                    var indexer = await TryCreateIndexer(dataSource, index);
                    break;
                }
                catch (Microsoft.Rest.Azure.CloudException ex) // only catch these dumb exceptions
                {
                    logger.LogCritical("Failed to created Amohora Index", ex);
                    if (attempts > maxAttempts)
                    {
                        logger.LogCritical("Max Attempts reached", ex);
                        throw ex;
                    }
                    await Task.Delay(500); // wait half a second before retrying
                }
            }

            var timeTaken = System.DateTime.Now - startTime;
            isCreatingIndex = false;
            isInitialised = true;
            logger.LogInformation($"{nameof(CreateAmphoraIndexAsync)} took {timeTaken.TotalSeconds} and {attempts} attempts");
        }

        private async Task<Index> TryCreateIndex()
        {
            Index index = new AmphoraSearchIndex();
            index.Validate();

            if (!await serviceClient.Indexes.ExistsAsync(index.Name))
            {
                index = await serviceClient.Indexes.CreateOrUpdateAsync(index);
            }
            return index;
        }

        private async Task<DataSource> TryCreateDatasource()
        {
            var query = "SELECT * FROM c WHERE c.Discriminator = 'AmphoraModel' AND c._ts > @HighWaterMark ORDER BY c._ts";
            var cosmosDbConnectionString = cosmosOptions.CurrentValue.GenerateConnectionString(cosmosOptions.CurrentValue.PrimaryReadonlyKey);
            var deletionPolicy = new SoftDeleteColumnDeletionDetectionPolicy(nameof(Entity.IsDeleted), "true");
            var dataSource = DataSource.CosmosDb("cosmos",
                                                 cosmosDbConnectionString,
                                                 nameof(AmphoraContext),
                                                 query, 
                                                 true,
                                                 deletionPolicy, 
                                                 $"Created by C# code {nameof(AzureSearchInitialiser)}");
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
                        {"assumeOrderByHighWaterMarkColumn", true}
                    }
                },
                FieldMappings = new List<FieldMapping> {
                    // encodes a field for use as the index key
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