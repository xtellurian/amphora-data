using System;
using System.Threading.Tasks;
using Amphora.Api.Options;
using Amphora.Common.Configuration.Options;
using Microsoft.Azure.Search;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Services.Azure
{
    public abstract class SearchInitialiserBase
    {
        protected readonly ILogger<SearchInitialiserBase> logger;
        protected readonly IOptionsMonitor<CosmosOptions> cosmosOptions;
        protected readonly SearchServiceClient serviceClient;
        protected bool isCreating;
        protected bool isInitialised;

        public SearchInitialiserBase(ILogger<SearchInitialiserBase> logger,
                                     IOptionsMonitor<AzureSearchOptions> options,
                                     IOptionsMonitor<CosmosOptions> cosmosOptions)
        {
            this.logger = logger;
            this.cosmosOptions = cosmosOptions;
            this.serviceClient = new SearchServiceClient(options.CurrentValue.Name, new SearchCredentials(options.CurrentValue.PrimaryKey));
        }

        protected abstract string IndexerName { get; }
        public abstract Task CreateIndexAsync();

        public async Task<bool> TryIndex()
        {
            // ensure created:
            if (!await serviceClient.Indexers.ExistsAsync(IndexerName))
            {
                logger.LogInformation($"Index request resulted in creation of full index.");
                await CreateIndexAsync();
            }

            try
            {
                await serviceClient.Indexers.RunAsync(IndexerName);
                return true;
            }
            catch (System.Exception ex)
            {
                logger.LogError($"TryIndex failed for indexer {IndexerName}", ex);
                return false;
            }
        }

        protected async Task TryNTimes(Func<Task> t, int maxAttempts = 5)
        {
            int attempts = 0;
            while (true)
            {
                try
                {
                    await t();
                    break;
                }
                catch (Microsoft.Rest.Azure.CloudException ex) // only catch these dumb exceptions
                {
                    logger.LogCritical("Failed to created Amphora Index", ex);
                    if (attempts++ > maxAttempts)
                    {
                        logger.LogCritical("Max Attempts reached", ex);
                        throw ex;
                    }

                    await Task.Delay(500); // wait half a second before retrying
                }
            }
        }

        protected async Task WaitWhileCreating()
        {
            while (isCreating)
            {
                // we need to wait for it to be set to false, then return
                logger.LogInformation($"Waiting for index to be created");
                await Task.Delay(1000);
            }
        }

        protected void OnStartInitialising()
        {
            isCreating = true;
        }

        protected void OnFinishInitialising()
        {
            isCreating = false;
            isInitialised = true;
        }
    }
}