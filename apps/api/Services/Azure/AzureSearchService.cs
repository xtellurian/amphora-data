using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.AzureSearch;
using Amphora.Api.Models.Search;
using Amphora.Api.Options;
using Amphora.Common.Models.Amphorae;
using AutoMapper;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Services.Azure
{
    public class AzureSearchService : ISearchService
    {
        private bool IsCreatingIndex;
        private readonly SearchServiceClient serviceClient;
        private readonly IOptionsMonitor<CosmosOptions> cosmosOptions;
        private readonly ILogger<AzureSearchService> logger;
        private readonly IMapper mapper;
        
        private const string indexerName = "amphora-indexer";

        public AzureSearchService(
            IOptionsMonitor<AzureSearchOptions> options,
            IOptionsMonitor<CosmosOptions> cosmosOptions,
            ILogger<AzureSearchService> logger,
            IMapper mapper)
        {
            this.serviceClient = new SearchServiceClient(options.CurrentValue.Name, new SearchCredentials(options.CurrentValue.PrimaryKey));
            this.cosmosOptions = cosmosOptions;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task CreateAmphoraIndexAsync()
        {
            IsCreatingIndex = true;
            logger.LogWarning("Recreating the Amphora index");

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
            await serviceClient.Indexes.DeleteAsync(index.Name);
            index = await serviceClient.Indexes.CreateOrUpdateAsync(index);

            var indexer = new Indexer(indexerName, dataSource.Name, index.Name)
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

            await serviceClient.Indexers.DeleteAsync(indexer.Name);
            indexer = await serviceClient.Indexers.CreateOrUpdateAsync(indexer);
            IsCreatingIndex = false;
        }

        public async Task Reindex()
        {
            if (!await serviceClient.Indexers.ExistsAsync(indexerName))
            {
                await CreateAmphoraIndexAsync();
            }
            else
            {
                var indexer = await serviceClient.Indexers.GetAsync(indexerName);
                await serviceClient.Indexers.RunAsync(indexer.Name);
            }
        }

        public async Task<EntitySearchResult<AmphoraModel>> SearchAmphora(string searchText, Models.Search.SearchParameters parameters)
        {
            if (!await serviceClient.Indexes.ExistsAsync(AmphoraSearchIndex.IndexName) && !IsCreatingIndex)
            {
                logger.LogWarning($"{AmphoraSearchIndex.IndexName} does not exist. Creating now");
                try
                {
                    await this.CreateAmphoraIndexAsync();
                }
                catch (Exception ex)
                {
                    logger.LogCritical($"Failed to created {AmphoraSearchIndex.IndexName}", ex);
                    throw;
                }
            }
            else if(IsCreatingIndex)
            {
                await Task.Delay(2000); // just wait 2 second because the index is probably being created right now
            }

            var indexClient = serviceClient.Indexes.GetClient(AmphoraSearchIndex.IndexName);

            var results = await indexClient.Documents.SearchAsync<AmphoraModel>(searchText, parameters);

            return mapper.Map<EntitySearchResult<AmphoraModel>>(results);
        }
    }
}