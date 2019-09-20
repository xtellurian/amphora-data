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
        private readonly IAzureSearchInitialiser searchInitialiser;
        private readonly ILogger<AzureSearchService> logger;
        private readonly IMapper mapper;
        
        

        public AzureSearchService(
            IOptionsMonitor<AzureSearchOptions> options,
            IAzureSearchInitialiser searchInitialiser,
            ILogger<AzureSearchService> logger,
            IMapper mapper)
        {
            this.serviceClient = new SearchServiceClient(options.CurrentValue.Name, new SearchCredentials(options.CurrentValue.PrimaryKey));
            this.searchInitialiser = searchInitialiser;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task<EntitySearchResult<AmphoraModel>> SearchAmphora(string searchText, Models.Search.SearchParameters parameters)
        {
            if (!await serviceClient.Indexes.ExistsAsync(AmphoraSearchIndex.IndexName) && !IsCreatingIndex)
            {
                logger.LogWarning($"{AmphoraSearchIndex.IndexName} does not exist. Creating now");
                try
                {
                    await this.searchInitialiser.CreateAmphoraIndexAsync();
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