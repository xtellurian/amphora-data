using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Identity.Integration
{
    [Collection(nameof(IdentityFixtureCollection))]
    public class IdentityPagesTests : IdentityIntegrationTestBase
    {
        public IdentityPagesTests(WebApplicationFactory<Amphora.Identity.Startup> factory) : base(factory)
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