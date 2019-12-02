using System.Threading.Tasks;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Contracts
{
    public interface IMapService
    {
        System.Threading.Tasks.Task<Common.Models.AzureMaps.FuzzySearchResponse> FuzzySearchAsync(string query);
        Task<byte[]> GetStaticMapImageAsync(GeoLocation location, int height = 512, int width = 512);
    }
}