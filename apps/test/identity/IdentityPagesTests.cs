using System.Threading.Tasks;
using Amphora.Tests.Setup;
using Xunit;

namespace Amphora.Tests.Identity.Integration
{
    [Collection(nameof(IdentityFixtureCollection))]
    public class IdentityPagesTests : IdentityInMemoryIntegrationTestBase
    {
        public IdentityPagesTests(InMemoryIdentityWebApplicationFactory factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("/Account/Edit")]
        public async Task CanLoadPage(string path)
        {
            var (adminClient, adminUser) = await NewAuthenticatedUser();

            var response = await adminClient.GetAsync(path);
            await AssertHttpSuccess(response);
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }
    }
}