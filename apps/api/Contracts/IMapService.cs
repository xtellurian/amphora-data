namespace Amphora.Api.Contracts
{
    public interface IMapService
    {
        System.Threading.Tasks.Task<Common.Models.AzureMaps.FuzzySearchResponse> FuzzySearchAsync(string query);
        System.Threading.Tasks.Task<byte[]> GetStaticMapImageAsync(Common.Models.Amphorae.GeoLocation location);
    }
}