using System.Threading.Tasks;
using Amphora.Api.Models.Search;
using Amphora.Api.Options;
using Amphora.Api.Services.Azure;
using Amphora.Tests.Unit;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Amphora.Tests.Services
{
    public class AzureSearchTests: UnitTestBase
    {
        private CosmosOptions cosmosOptions;
        private AzureSearchOptions searchOptions;

        public AzureSearchTests()
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
            
        }
        // these need keys are are hard to run automatically
        // [Fact]
        public async Task CreateIndex_Success()
        {
            // Setup
            var sut = new AzureSearchService( 
                Mock.Of<IOptionsMonitor<AzureSearchOptions>>(_ => _.CurrentValue == searchOptions) , 
                Mock.Of<IOptionsMonitor<CosmosOptions>>(_ => _.CurrentValue == cosmosOptions),
                Mapper);
            // act
            await sut.CreateAmphoraIndexAsync();
        }

        // [Fact]
        public async Task SearchAmphora_Success()
        {

             var sut = new AzureSearchService( 
                Mock.Of<IOptionsMonitor<AzureSearchOptions>>(_ => _.CurrentValue == searchOptions) , 
                Mock.Of<IOptionsMonitor<CosmosOptions>>(_ => _.CurrentValue == cosmosOptions),
                Mapper);
            
            var parameters = new SearchParameters();
            var result = await sut.SearchAmphora("", parameters);

            Assert.All(result.Results, r => Assert.NotNull(r.Entity));
        }
    }
}