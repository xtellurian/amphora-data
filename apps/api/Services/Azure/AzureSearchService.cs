using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.AzureSearch;
using Amphora.Api.Models.Search;
using Amphora.Api.Options;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Amphorae;
using AutoMapper;
using Microsoft.Azure.Search;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Services.Azure
{
    public class AzureSearchService : ISearchService
    {
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

        public async Task<bool> TryIndex()
        {
            return await this.searchInitialiser.TryIndex();
        }

        public async Task<long?> SearchAmphoraCount(string searchText, Models.Search.SearchParameters parameters)
        {
            await this.searchInitialiser.CreateAmphoraIndexAsync();
            parameters.WithTotalResultCount();
            var indexClient = serviceClient.Indexes.GetClient(AmphoraSearchIndex.IndexName);
            try
            {
                var results = await indexClient.Documents.SearchAsync<AmphoraModel>(searchText, parameters);
                return results.Count;
            }
            catch (Microsoft.Rest.Azure.CloudException ex)
            {
                logger.LogWarning($"{indexClient.IndexName} threw on Search Count Async.", ex);
                return null;
            }
        }

        public async Task<EntitySearchResult<AmphoraModel>> SearchAmphora(string searchText, Models.Search.SearchParameters parameters)
        {
            await this.searchInitialiser.CreateAmphoraIndexAsync();

            var indexClient = serviceClient.Indexes.GetClient(AmphoraSearchIndex.IndexName);
            try
            {
                var results = await indexClient.Documents.SearchAsync<AmphoraModel>(searchText, parameters);

                return mapper.Map<EntitySearchResult<AmphoraModel>>(results);
            }
            catch (Microsoft.Rest.Azure.CloudException ex)
            {
                logger.LogWarning($"{indexClient.IndexName} threw on Search Async. Retrying in 5 seconds...", ex);
            }

            await Task.Delay(1000 * 5);
            try
            {
                var results_secondTry = await indexClient.Documents.SearchAsync<AmphoraModel>(searchText, parameters);

                return mapper.Map<EntitySearchResult<AmphoraModel>>(results_secondTry);
            }
            catch (Microsoft.Rest.Azure.CloudException ex)
            {
                logger.LogError($"{indexClient.IndexName} threw on second try Search Async.", ex);
                throw ex;
            }
        }
    }
}