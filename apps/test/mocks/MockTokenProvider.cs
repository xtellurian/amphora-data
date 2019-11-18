using System.Threading.Tasks;
using Amphora.Common.Contracts;

namespace Amphora.Tests.Mocks
{
    class MockTokenProvider : IAzureServiceTokenProvider
    {
        public static string Token => "tokenz";

        public Task<string> GetAccessTokenAsync(string resource, string tenantId = null)
        {
            return Task<string>.Factory.StartNew(() =>
            {
                return Token;
            });
        }
    }
}