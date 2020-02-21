using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.AzureSearch;
using Amphora.Api.Models.Search;
using Amphora.Api.Options;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.DataRequests;
using Amphora.Common.Models.Organisations;
using AutoMapper;
using Microsoft.Azure.Search;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Services.Azure
{
    public class AzureSearchService : ISearchService
    {
        private readonly SearchServiceClient serviceClient;
        private readonly IAzureSearchInitialiser<AmphoraModel> amphoraSearchInitialiser;
        private readonly IAzureSearchInitialiser<DataRequestModel> dataRequestSearchInitialiser;
        private readonly IAzureSearchInitialiser<OrganisationModel> organisationSearchInitialiser;
        private readonly ILogger<AzureSearchService> logger;
        private readonly IMapper mapper;

        public AzureSearchService(
            IOptionsMonitor<AzureSearchOptions> options,
            IAzureSearchInitialiser<AmphoraModel> amphoraSearchInitialiser,
            IAzureSearchInitialiser<DataRequestModel> dataRequestSearchInitialiser,
            IAzureSearchInitialiser<OrganisationModel> organisationSearchInitialiser,
            ILogger<AzureSearchService> logger,
            IMapper mapper)
        {
            this.serviceClient = new SearchServiceClient(options.CurrentValue.Name, new SearchCredentials(options.CurrentValue.PrimaryKey));
            this.amphoraSearchInitialiser = amphoraSearchInitialiser;
            this.dataRequestSearchInitialiser = dataRequestSearchInitialiser;
            this.organisationSearchInitialiser = organisationSearchInitialiser;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task<bool> TryIndex()
        {
            var t1 = this.amphoraSearchInitialiser.TryIndex();
            var t2 = this.dataRequestSearchInitialiser.TryIndex();
            var t3 = this.organisationSearchInitialiser.TryIndex();
            var res = await Task.WhenAll(t1, t2, t3);
            return t1.Result && t2.Result && t3.Result;
        }

        public async Task<long?> SearchAmphoraCount(string searchText, Models.Search.SearchParameters parameters)
        {
            await this.amphoraSearchInitialiser.CreateIndexAsync();
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
            await this.amphoraSearchInitialiser.CreateIndexAsync();

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

        public async Task<EntitySearchResult<T>> SearchAsync<T>(string searchText, Models.Search.SearchParameters parameters) where T : ISearchable
        {
            using (var client = await GetSearchClientAsync<T>())
            {
                try
                {
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

        private async Task<ISearchIndexClient> GetSearchClientAsync<T>() where T : ISearchable
        {
            if (typeof(T) == typeof(AmphoraModel))
            {
                await this.amphoraSearchInitialiser.CreateIndexAsync();
                return serviceClient.Indexes.GetClient(AmphoraSearchIndex.IndexName);
            }
            else if (typeof(T) == typeof(DataRequestModel))
            {
                await this.dataRequestSearchInitialiser.CreateIndexAsync();
                return serviceClient.Indexes.GetClient(DataRequestSearchIndex.IndexName);
            }
            else if (typeof(T) == typeof(OrganisationModel))
            {
                await this.organisationSearchInitialiser.CreateIndexAsync();
                return serviceClient.Indexes.GetClient(OrganisationSearchIndex.IndexName);
            }
            else
            {
                throw new System.ArgumentException($"Unknown Search Type, {typeof(T)}");
            }
        }
    }
}
