using System.Net.Http;
using System.Threading.Tasks;
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
    }
}