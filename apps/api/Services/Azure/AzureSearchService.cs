using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Common.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Services.Azure
{
    public class AzureSearchService : ISearchService
    {
        private readonly SearchServiceClient serviceClient;
        private readonly IOptionsMonitor<CosmosOptions> cosmosOptions;

        public AzureSearchService(IOptionsMonitor<AzureSearchOptions> options, IOptionsMonitor<CosmosOptions> cosmosOptions)
        {
            this.serviceClient = new SearchServiceClient(options.CurrentValue.Name, new SearchCredentials(options.CurrentValue.PrimaryKey));
            this.cosmosOptions = cosmosOptions;
        }

        public async Task CreateAmphoraIndexAsync()
        {
            var query = "SELECT * FROM c WHERE STARTSWITH(c.id, 'Amphora|') AND c._ts > @HighWaterMark";
            var cosmosDbConnectionString = cosmosOptions.CurrentValue.ConnectionString;
            var dataSource = DataSource.CosmosDb("Cosmos",
                                                 cosmosDbConnectionString,
                                                 cosmosOptions.CurrentValue.Database,
                                                 query);

            dataSource = await serviceClient.DataSources.CreateOrUpdateAsync(dataSource);
            var fields = new List<Field>();
            // add name
            var nameField = new Field(nameof(AmphoraModel.Name), DataType.String);
            nameField.IsSearchable = true;
            nameField.IsRetrievable = true;
            fields.Add(nameField);
            // add about
            var descriptionField = new Field(nameof(AmphoraModel.Description), DataType.String);
            descriptionField.IsSearchable = true;
            fields.Add(descriptionField);
            // add price
            var priceField = new Field(nameof(AmphoraModel.Price), DataType.Double);
            priceField.IsSortable = true;
            priceField.IsFacetable = true;
            priceField.IsFilterable = true;
            fields.Add(priceField);
            // add isPubic
            var isPublicField = new Field(nameof(AmphoraModel.IsPublic), DataType.Boolean);
            isPublicField.IsFilterable = true;
            fields.Add(isPublicField);

            var index = new Index()
            {
                Name = "AmphoraIndex",
                Fields = fields
            };

            index = await serviceClient.Indexes.CreateOrUpdateAsync(index);

            var indexer = new Indexer("AmphoraIndexer", dataSource.Name, index.Name);
            indexer = await serviceClient.Indexers.CreateOrUpdateAsync(indexer);

        }

        public Task SearchAmphora(Amphora.Api.Models.Search.SearchParameters parameters)
        {
            var indexClient = serviceClient.Indexes.GetClient("AmphoraIndex");

            var results = indexClient.Documents.Search<AmphoraModel>("Atlanta", parameters);
            return Task.CompletedTask;
        }
    }
}