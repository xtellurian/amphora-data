using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Identity.Integration
{
    [Collection(nameof(IdentityFixtureCollection))]
    [Trait("Category", "Identity")]
    public class CosmosTests
    {
        private readonly WebApplicationFactory<Amphora.Identity.Startup> factory;

        public CosmosTests(WebApplicationFactory<Amphora.Identity.Startup> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task CanCreateUser_AsAnonymous_AndGetToken_AndDelete()
        {
            var client = factory.CreateClient();
            await CRUDUserTests.Run_CanCreateUser_AsAnonymous_AndGetToken_AndDelete(client);
        }
    }
}