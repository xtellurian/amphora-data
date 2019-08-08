using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    public class TemporaCRUDTests : IClassFixture<WebApplicationFactory<Amphora.Api.Startup>>
    {
        private readonly WebApplicationFactory<Amphora.Api.Startup> _factory;

        public TemporaCRUDTests(WebApplicationFactory<Amphora.Api.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/temporae")]
        public async Task Put_Creates_HappyPath(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            var a = Helpers.EntityLibrary.GetValidTempora();
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");

            // Act
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await client.PutAsync(url, requestBody);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody);
            var b = JsonConvert.DeserializeObject<Amphora.Common.Models.Tempora>(responseBody);
            Assert.NotNull(b.Id);
            Assert.Equal(a.Description, b.Description);
            Assert.Equal(a.Price, b.Price);
            Assert.Equal(a.Title, b.Title);
        }
        [Theory]
        [InlineData("/api/temporae")]
        public async Task Put_Creates_InvalidModel(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            var a = Helpers.EntityLibrary.GetInvalidTempora();
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");

            // Act
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await client.PutAsync(url, requestBody);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest , response.StatusCode);
        }

        [Theory]
        [InlineData("/api/temporae")]
        public async Task Get_Lists_HappyPath(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            await this.Put_Creates_HappyPath(url); // create an amphora for the test
            // Act
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody);
            var b = JsonConvert.DeserializeObject<List<Amphora.Common.Models.Tempora>>(responseBody);
            Assert.True(b.Count > 0);
        }

        [Theory]
        [InlineData("/api/temporae")]
        public async Task Get_Reads_HappyPath(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            var a = Helpers.EntityLibrary.GetValidTempora();
            var requestBody = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var createResponse = await client.PutAsync(url, requestBody);
            createResponse.EnsureSuccessStatusCode();
            var b = JsonConvert.DeserializeObject<Amphora.Common.Models.Tempora>(await createResponse.Content.ReadAsStringAsync());

            // Act
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await client.GetAsync($"{url}/{b.Id}");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody);
            var c = JsonConvert.DeserializeObject<Amphora.Common.Models.Tempora>(responseBody);
            Assert.Equal(b.Id, c.Id);
            Assert.Equal(b.Description, c.Description);
            Assert.Equal(b.Price, c.Price);
            Assert.Equal(b.Title, c.Title);
        }
    
    }
}