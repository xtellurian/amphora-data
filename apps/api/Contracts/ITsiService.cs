using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.TimeSeriesInsights.Models;

namespace Amphora.Api.Contracts
{
    public interface ITsiService
    {
        Task<QueryResultPage> FullSet(string id, string property, System.DateTime start, System.DateTime end);
        // string GetDataAccessFqdn();
        // Task<QueryResponse> WeeklyAverageAsync(string id, string property, System.DateTime start, System.DateTime end);
    }
}