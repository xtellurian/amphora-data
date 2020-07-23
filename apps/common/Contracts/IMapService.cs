using System.Net;
using System.Threading.Tasks;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.AzureMaps;

namespace Amphora.Common.Contracts
{
    public interface IMapService
    {
        Task<FuzzySearchResponse> FuzzySearchAsync(string query, string? countrySet = null);
        Task<GeoLocation> GetPositionFromIp(string ipAddress);
        Task<GeoLocation> GetPositionFromIp(IPAddress ipAddress);
        Task<byte[]> GetStaticMapImageAsync(GeoLocation location, int height = 512, int width = 512);
        Task<string?> GetSubscriptionKeyAsync();
    }
}