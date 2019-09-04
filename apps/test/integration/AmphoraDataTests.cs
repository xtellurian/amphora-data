using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    public class AmphoraDataTests : IntegrationTestBase, IClassFixture<WebApplicationFactory<Amphora.Api.Startup>>
    {

        public AmphoraDataTests(WebApplicationFactory<Amphora.Api.Startup> factory): base(factory)
        {
        }

        [Theory]
        [InlineData("/api/amphorae")]
        public async Task Post_UploadAndDownload_HappyPath(string url)
        {
            // Arrange
            var client = await GetAuthenticatedClientAsync();
            
            var amphora = await CreateAmphoraAsync(client, url);
            var generator = new Helpers.RandomBufferGenerator(1024);
            var content = generator.GenerateBufferFromSeed(1024);
            var requestBody = new ByteArrayContent(content);
            var name = Guid.NewGuid().ToString();

            // Act
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var uploadResponse = await client.PostAsync($"{url}/{amphora.Id}/upload?name={name}", requestBody);
            uploadResponse.EnsureSuccessStatusCode();

            var downloadResponse = await client.GetAsync($"{url}/{amphora.Id}/download?name={name}");
            downloadResponse.EnsureSuccessStatusCode();

            // Assert
            Assert.Equal(content, await downloadResponse.Content.ReadAsByteArrayAsync());

            // cleanup
            await DeleteAmphora(client, amphora.Id);
        }
        [Theory]
        [InlineData("/api/amphorae")]
        public async Task Post_UploadToAmphora_MissingEntity(string url)
        {
            // Arrange
            var client = await GetAuthenticatedClientAsync();
            var generator = new Helpers.RandomBufferGenerator(1024);
            var content = generator.GenerateBufferFromSeed(1024);
            var requestBody = new ByteArrayContent(content);

            // Act
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var fillResponse = await client.PostAsync($"{url}/{Guid.NewGuid()}/upload", requestBody);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, fillResponse.StatusCode);
        }

        private async Task<Amphora.Common.Models.Amphora> CreateAmphoraAsync(HttpClient client, string url)
        {
            var amphora = Helpers.EntityLibrary.GetAmphora();
            // create an amphora for us to work with
            var createResponse = await client.PostAsync(url,
                new StringContent(JsonConvert.SerializeObject(amphora), Encoding.UTF8, "application/json")
                );
            createResponse.EnsureSuccessStatusCode();
            amphora = JsonConvert.DeserializeObject<Amphora.Common.Models.Amphora>(
                await createResponse.Content.ReadAsStringAsync()
                );
            return amphora;
        }

        private async Task DeleteAmphora(HttpClient client, string id)
        {
            var deleteResponse = await client.DeleteAsync($"/api/amphorae/{id}");
            var response = await client.GetAsync($"api/amphorae/{id}");
            deleteResponse.EnsureSuccessStatusCode();
        }
    }
}