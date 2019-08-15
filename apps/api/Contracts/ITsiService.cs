using System.Net.Http;
using System.Threading.Tasks;
using TimeSeriesInsightsClient.Queries;

namespace Amphora.Api.Contracts
{
    public interface ITsiService
    {
        System.Threading.Tasks.Task<string> GetAccessTokenAsync();
        string GetDataAccessFqdn();
        Task<HttpResponseMessage> ProxyQueryAsync(string uri, System.Net.Http.HttpContent content);
        Task<QueryResponse> WeeklyAverageAsync(string id, string property, System.DateTime start, System.DateTime end);
    }
}