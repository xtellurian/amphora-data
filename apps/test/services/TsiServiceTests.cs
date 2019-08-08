using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Tests.Mocks;
using Xunit;

namespace Amphora.Tests.Unit
{
    public class TsiServiceTests
    {
        private readonly ITsiService sut;
        private readonly IAzureServiceTokenProvider tokenProvider;

        public TsiServiceTests(ITsiService tsiService, IAzureServiceTokenProvider tokenProvider)
        {
            this.sut = tsiService;
            this.tokenProvider = tokenProvider;
        }

        [Fact]
        public async Task GetAccessToken_Happy()
        {
            var token = await sut.GetAccessTokenAsync();
            var providedToken = await tokenProvider.GetAccessTokenAsync("anything for now");
            Assert.Equal(providedToken, token);
        }
    }
}