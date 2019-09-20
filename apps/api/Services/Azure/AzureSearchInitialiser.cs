using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.AzureSearch;
using Amphora.Api.Options;
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
        public string IndexerName => "amphora-indexer";

        public async Task CreateAmphoraIndexAsync()
        {
            var startTime = System.DateTime.Now;
            if(isInitialised) return;
            if (await serviceClient.Indexes.ExistsAsync(AmphoraSearchIndex.IndexName))
            {
                logger.LogInformation($"{AmphoraSearchIndex.IndexName} already exists. Waiting 5 seconds");
                // might still be creating, wait 5 seconds
                await Task.Delay(1000 * 5);
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
            try
            {
                var query = "SELECT * FROM c WHERE STARTSWITH(c.id, 'Amphora|') AND c._ts > @HighWaterMark ORDER BY c._ts";
                var cosmosDbConnectionString = cosmosOptions.CurrentValue.GenerateConnectionString(cosmosOptions.CurrentValue.PrimaryReadonlyKey);
                var dataSource = DataSource.CosmosDb("cosmos",
                                                     cosmosDbConnectionString,
                                                     "Amphora",
                                                     query);
                dataSource.Validate();
                dataSource = await serviceClient.DataSources.CreateOrUpdateAsync(dataSource);

                Index index = new AmphoraSearchIndex();
                index.Validate();
                if (!await serviceClient.Indexes.ExistsAsync(index.Name))
                {

                    index = await serviceClient.Indexes.CreateOrUpdateAsync(index);
                }

                var indexer = new Indexer(IndexerName, dataSource.Name, index.Name)
                {
                    Schedule = new IndexingSchedule(System.TimeSpan.FromHours(1)),
                    Parameters = new IndexingParameters
                    {
                        Configuration = new Dictionary<string, object>
                    {
                        {"assumeOrderByHighWaterMarkColumn", true}
                    }
                    }
                };
                indexer.Validate();
                if (!await serviceClient.Indexers.ExistsAsync(indexer.Name))
                {
                    indexer = await serviceClient.Indexers.CreateOrUpdateAsync(indexer);
                }
            }
            catch (System.Exception ex)
            {
                logger.LogCritical("Failed to created Amohora Index", ex);
                throw;
            }
            finally
            {
                var timeTaken = System.DateTime.Now - startTime;
                logger.LogInformation($"{nameof(CreateAmphoraIndexAsync)} took {timeTaken.TotalSeconds}");
                isCreatingIndex = false;
                isInitialised = true;
            }
        }
    }
}