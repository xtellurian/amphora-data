using System.Threading.Tasks;

namespace Amphora.Api.Contracts
{
    public interface IAzureServiceTokenProvider
    {
        Task<string> GetAccessTokenAsync(string resource, string tenantId = null);
    }
}