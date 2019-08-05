using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amphora.Common.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests.Integration
{
    public class AmphoraCRUDTests : IClassFixture<WebApplicationFactory<Amphora.Api.Startup>>
    {
         private readonly WebApplicationFactory<Amphora.Api.Startup> _factory;

        public AmphoraCRUDTests(WebApplicationFactory<Amphora.Api.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/amphorae")]
        public async Task Put_CreatesAmphora(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            var a = Helpers.AmphoraLibrary.GetValidAmphora();
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
            var b = JsonConvert.DeserializeObject<Amphora.Common.Models.Amphora>(responseBody);
            Assert.NotNull(b.Id);
            Assert.Equal(a.Bounded, b.Bounded);
            Assert.Equal(a.Description, b.Description);
            Assert.Equal(a.Price, b.Price);
            Assert.Equal(a.SchemaId, b.SchemaId);
            Assert.Equal(a.Title, b.Title);
        }

        [Theory]
        [InlineData("/api/amphorae")]
        public async Task Get_ListsAmphorae(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            await this.Put_CreatesAmphora(url); // create an amphora for the test
            // Act
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseBody);
            var b = JsonConvert.DeserializeObject<List<Amphora.Common.Models.Amphora>>(responseBody);
            Assert.True(b.Count > 0);
        }
    
    }
}