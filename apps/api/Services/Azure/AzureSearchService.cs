using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Search;
using Amphora.Api.Options;
using Amphora.Common.Models;
using AutoMapper;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Services.Azure
{
    public class AzureSearchService : ISearchService
    {
        private readonly SearchServiceClient serviceClient;
        private readonly IOptionsMonitor<CosmosOptions> cosmosOptions;
        private readonly IMapper mapper;
        private const string indexName = "amphora-index";

        public AzureSearchService(
            IOptionsMonitor<AzureSearchOptions> options,
            IOptionsMonitor<CosmosOptions> cosmosOptions,
            IMapper mapper)
        {
            this.serviceClient = new SearchServiceClient(options.CurrentValue.Name, new SearchCredentials(options.CurrentValue.PrimaryKey));
            this.cosmosOptions = cosmosOptions;
            this.mapper = mapper;
        }

        public async Task CreateAmphoraIndexAsync()
        {
            var query = "SELECT * FROM c WHERE STARTSWITH(c.id, 'Amphora|') AND c._ts > @HighWaterMark";
            var cosmosDbConnectionString = cosmosOptions.CurrentValue.PrimaryReadonlyKey;
            var dataSource = DataSource.CosmosDb("cosmos",
                                                 cosmosDbConnectionString,
                                                 "Amphora",
                                                 query);
            dataSource.Validate();

            dataSource = await serviceClient.DataSources.CreateOrUpdateAsync(dataSource);
            var fields = new List<Field>();

            fields.Add(new Field(nameof(AmphoraModel.Id), DataType.String));
            fields.Add(new Field(nameof(AmphoraModel.AmphoraId), DataType.String)
            {
                IsKey = true // key of AmphoraId because Id has a special charcter not allowed
            });
            // add name
            fields.Add(new Field(nameof(AmphoraModel.Name), DataType.String)
            {
                IsSearchable = true,
                IsRetrievable = true
            });
            // add about
            fields.Add(new Field(nameof(AmphoraModel.Description), DataType.String)
            {
                IsSearchable = true
            });
            // add price
            fields.Add(new Field(nameof(AmphoraModel.Price), DataType.Double)
            {
                IsRetrievable = true,
                IsSortable = true,
                IsFacetable = true,
                IsFilterable = true
            });

            // add isPubic
            fields.Add(new Field(nameof(AmphoraModel.IsPublic), DataType.Boolean)
            {
                IsFilterable = true
            });

            var index = new Index()
            {
                Name = indexName,
                Fields = fields
            };
            index.Validate();
            await serviceClient.Indexes.DeleteAsync(index.Name);
            index = await serviceClient.Indexes.CreateOrUpdateAsync(index);

            var indexer = new Indexer("amphora-indexer", dataSource.Name, index.Name)
            {
                Schedule = new IndexingSchedule(System.TimeSpan.FromHours(1))
            };
            indexer.Validate();

            await serviceClient.Indexers.DeleteAsync(indexer.Name);

            indexer = await serviceClient.Indexers.CreateOrUpdateAsync(indexer);


        }

        public async Task<EntitySearchResult<AmphoraModel>> SearchAmphora(string searchText, Models.Search.SearchParameters parameters)
        {
            var indexClient = serviceClient.Indexes.GetClient(indexName);

            var results = await indexClient.Documents.SearchAsync<AmphoraModel>(searchText, parameters);

            return mapper.Map<EntitySearchResult<AmphoraModel>>(results);

        }
    }
}