using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.AzureSearch;
using Amphora.Api.Models.Search;
using Amphora.Api.Options;
using Amphora.Common.Contracts;
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
            var t1 = this.searchInitialiser.TryIndex();
            var res = await Task.WhenAll(t1);
            return t1.Result;
        }

        public async Task<long?> SearchAmphoraCount(string searchText, Models.Search.SearchParameters parameters)
        {
            parameters.WithTotalResultCount();
            var results = await this.SearchAsync<AmphoraModel>(searchText, parameters);
            return results?.Count;
        }

        public async Task<EntitySearchResult<AmphoraModel>> SearchAmphora(string searchText, Models.Search.SearchParameters parameters)
        {
            return await this.SearchAsync<AmphoraModel>(searchText, parameters);
        }

        public async Task<EntitySearchResult<T>> SearchAsync<T>(string searchText, Models.Search.SearchParameters parameters = null) where T : ISearchable
        {
            using (var client = await GetSearchClientAsync())
            {
                try
                {
                    parameters ??= new SearchParameters();
                    var filterExpression = $"Discriminator eq '{typeof(T).Name}'";
                    parameters.Filter ??= "";
                    if (!parameters.Filter.Contains(filterExpression))
                    {
                        // add the filter
                        if (parameters.Filter.Length > 0)
                        {
                            parameters.Filter += " and ";
                        }

                        parameters.Filter += filterExpression;
                    }

                    var results = await client.Documents.SearchAsync<T>(searchText, parameters);
                    return mapper.Map<EntitySearchResult<T>>(results);
                }
                catch (Microsoft.Rest.Azure.CloudException ex)
                {
                    logger.LogWarning($"{client.IndexName} threw on Search Async.", ex);
                    return new EntitySearchResult<T>();
                }
            }
        }

        private async Task<ISearchIndexClient> GetSearchClientAsync()
        {
            await this.searchInitialiser.CreateIndexAsync();
            return serviceClient.Indexes.GetClient(UnifiedSearchIndex.IndexName);
        }
    }
}
