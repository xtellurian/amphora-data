using System.Threading.Tasks;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.AzureMaps;

namespace Amphora.Common.Contracts
{
    public interface IMapService
    {
        Task<FuzzySearchResponse> FuzzySearchAsync(string query);
        Task<byte[]> GetStaticMapImageAsync(GeoLocation location, int height = 512, int width = 512);
    }
}