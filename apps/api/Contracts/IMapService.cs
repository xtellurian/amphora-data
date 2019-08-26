namespace Amphora.Api.Contracts
{
    public interface IMapService
    {
        System.Threading.Tasks.Task<Common.Models.AzureMaps.FuzzySearchResponse> FuzzySearchAsync(string query);
    }
}