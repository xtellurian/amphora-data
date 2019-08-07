using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    public class AmphoraDataTests : IClassFixture<WebApplicationFactory<Amphora.Api.Startup>>
    {
        private readonly WebApplicationFactory<Amphora.Api.Startup> _factory;

        public AmphoraDataTests(WebApplicationFactory<Amphora.Api.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/amphorae")]
        public async Task Post_UploadToAmphora(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            var amphora = await CreateAmphoraAsync(client, url);
            var generator = new Helpers.RandomBufferGenerator(1024);
            var content = generator.GenerateBufferFromSeed(1024);
            var requestBody = new ByteArrayContent(content);

            // Act
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var fillResponse = await client.PostAsync($"{url}/{amphora.Id}/upload", requestBody);

            // Assert
            fillResponse.EnsureSuccessStatusCode();

        }

        private async Task<Amphora.Common.Models.Amphora> CreateAmphoraAsync(HttpClient client, string url)
        {
            var amphora = Helpers.AmphoraLibrary.GetValidAmphora();
            // create an amphora for us to work with
            var createResponse = await client.PutAsync(url,
                new StringContent(JsonConvert.SerializeObject(amphora), Encoding.UTF8, "application/json")
                );
            amphora = JsonConvert.DeserializeObject<Amphora.Common.Models.Amphora>(
                await createResponse.Content.ReadAsStringAsync()
                );
            return amphora;
        }
    }
}