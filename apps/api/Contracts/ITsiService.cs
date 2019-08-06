using System.Net.Http;
using System.Threading.Tasks;

namespace Amphora.Api.Contracts
{
    public interface ITsiService
    {
        System.Threading.Tasks.Task<string> GetAccessTokenAsync();
        string GetDataAccessFqdn();
        Task<HttpResponseMessage> ProxyQueryAsync(string uri, System.Net.Http.HttpContent content);
    }
}