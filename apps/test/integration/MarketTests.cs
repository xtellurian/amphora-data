using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration
{
    [Collection(nameof(IntegrationFixtureCollection))]
    public class MarketTests : IntegrationTestBase
    {
        public MarketTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {

        }


    }
}