using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration
{
    public class MarketTests : IntegrationTestBase, IClassFixture<WebApplicationFactory<Amphora.Api.Startup>>
    {
        public MarketTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {

        }


    }
}