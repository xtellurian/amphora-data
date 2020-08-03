using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Tests.Helpers;
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
        private const string IpAddressHeader = "X-ClientIP";
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
                    { IpAddressHeader, "10.10.10.10" } // fake IP address to be limited,
                }
            };
        }

        private async Task<HttpResponseMessage> MakeRequest(Persona persona, HttpRequestMessage m)
        {
            return await persona.Http.SendAsync(m);
        }

        [Theory]
        [InlineData("api/users/self")]
        public async Task Get_BurstIsRateLimited(string path)
        {
            var persona = await GetPersonaAsync(Personas.Other);
            var b = new UriBuilder(persona.Http.BaseAddress);
            b.Path = path;
            var start = DateTime.Now;

            var successfulTasks = new List<Task<HttpResponseMessage>>
            {
                MakeRequest(persona, CreateRequestMessage(b.Uri)),
                MakeRequest(persona, CreateRequestMessage(b.Uri)),
            };
            await Task.WhenAll(successfulTasks);
            var rateLimitedResponse = await MakeRequest(persona, CreateRequestMessage(b.Uri));
            var end = DateTime.Now;
            (end - start).TotalSeconds.Should().BeLessThan(5, "because this test needs to run in less than 10 seconds");

            var successfulResponses = successfulTasks.Select(_ => _.Result).ToList();
            successfulResponses.Where(_ => _.IsSuccessStatusCode).Should().HaveCount(2);
            rateLimitedResponse.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
            rateLimitedResponse.IsSuccessStatusCode.Should().BeFalse("because one should have been rate limited");
            var content = await rateLimitedResponse.Content.ReadAsStringAsync();
            System.Console.WriteLine(content);
        }
    }
}