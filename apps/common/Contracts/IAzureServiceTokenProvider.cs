using System.Threading.Tasks;

namespace Amphora.Common.Contracts
{
    public interface IAzureServiceTokenProvider
    {
        Task<string> GetAccessTokenAsync(string resource, string? tenantId = null);
    }
}