using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace Amphora.Tests
{
    public abstract class IntegrationTestBase
    {
        protected async Task AssertHttpSuccess(HttpResponseMessage response, string path = "?")
        {
            Assert.True(response.IsSuccessStatusCode, $"Path: {path} , Code: {response.StatusCode},"
            + $"Content: {await response.Content.ReadAsStringAsync()}");
        }

        protected async Task<T> AssertHttpSuccess<T>(HttpResponseMessage response, string path = "?")
        {
            var content = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, $"Path: {path} , Code: {response.StatusCode},"
            + $"Content: {content}");

            var obj = JsonConvert.DeserializeObject<T>(content);
            Assert.NotNull(obj);
            return obj;
        }
    }
}