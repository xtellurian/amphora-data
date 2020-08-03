using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Tests.Setup;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration
{
    [Collection(nameof(ApiFixtureCollection))]
    [Trait("Category", "RateLimit")]
    public class RateLimitTests : WebAppIntegrationTestBase
    {
        private const string IpAddressHeader = "X-Azure-ClientIP";
        public RateLimitTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        {
        }

        private HttpRequestMessage CreateRequestMessage(Uri uri)
        {
            return new HttpRequestMessage
            {
                RequestUri = uri,
                Method = HttpMethod.Get,
                Headers =
                {
                    { IpAddressHeader, "10.10.10.10" } // fake IP address to be limitted,
                }
            };
        }

        [Theory]
        [InlineData("api/amphorae/")]
        public async Task Get_BurstIsRateLimited(string path)
        {
            var persona = await GetPersonaAsync(Personas.Other);
            var b = new UriBuilder(persona.Http.BaseAddress);
            b.Path = path;
            // first should succeed
            var start = DateTime.Now;
            var firstResponse = await persona.Http.SendAsync(CreateRequestMessage(b.Uri));
            firstResponse.IsSuccessStatusCode.Should().BeTrue();
            // second should succeed
            var secondResponse = await persona.Http.SendAsync(CreateRequestMessage(b.Uri));
            secondResponse.IsSuccessStatusCode.Should().BeTrue();
            // third should be rate limitted and return 429
            var thirdResponse = await persona.Http.SendAsync(CreateRequestMessage(b.Uri));
            var end = DateTime.Now;
            (end - start).TotalSeconds.Should().BeLessThan(1, "because this test needs to run in less than 1 second");
            thirdResponse.IsSuccessStatusCode.Should().BeFalse("because we set the number in 1 seconds to be 2");
        }
    }
}