using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Search;
using Amphora.Api.Options;
using Amphora.Api.Services.Azure;
using Amphora.Tests.Unit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Amphora.Tests.Services
{
    public class AzureSearchTests: UnitTestBase
    {
        private readonly ILogger<AzureSearchService> azureSearchLogger;
        private readonly ILogger<AzureSearchInitialiser> initLogger;
        private CosmosOptions cosmosOptions;
        private AzureSearchOptions searchOptions;

        public AzureSearchTests(ILogger<AzureSearchService> azureSearchLogger, ILogger<AzureSearchInitialiser> initLogger)
        {
            this.cosmosOptions = new CosmosOptions()
            {
                PrimaryReadonlyKey = "",
                Database = "",
                Endpoint = ""
            };
            this.searchOptions = new AzureSearchOptions()
            {
                Name = "",
                PrimaryKey = ""
            };
            this.azureSearchLogger = azureSearchLogger;
            this.initLogger = initLogger;
        }
        // these need keys are are hard to run automatically
        // [Fact]
        public async Task CreateIndex_Success()
        {
            // Setup
            var sut = new AzureSearchInitialiser( 
                initLogger,
                Mock.Of<IOptionsMonitor<AzureSearchOptions>>(_ => _.CurrentValue == searchOptions) , 
                Mock.Of<IOptionsMonitor<CosmosOptions>>(_ => _.CurrentValue == cosmosOptions));
            // act
            await sut.CreateAmphoraIndexAsync();
        }

        // [Fact]
        public async Task SearchAmphora_Success()
        {

             var sut = new AzureSearchService( 
                Mock.Of<IOptionsMonitor<AzureSearchOptions>>(_ => _.CurrentValue == searchOptions) , 
                Mock.Of<IAzureSearchInitialiser>(),
                azureSearchLogger,
                Mapper);
            
            var parameters = new SearchParameters();
            var result = await sut.SearchAmphora("", parameters);

            Assert.All(result.Results, r => Assert.NotNull(r.Entity));
        }
    }
}