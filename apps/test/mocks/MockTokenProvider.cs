using System.Threading.Tasks;
using Amphora.Common.Contracts;

namespace Amphora.Tests.Mocks
{
    internal class MockTokenProvider : IAzureServiceTokenProvider
    {
        public static string Token => "tokenz";

        public Task<string> GetAccessTokenAsync(string resource, string tenantId = null)
        {
            return Task.FromResult(Token);
        }
    }
}