using System.Threading.Tasks;
using Amphora.Api.Models.Search;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Contracts
{
    public interface ISearchService
    {
        // Task Reindex(); - can only run once every 180 seconds (i give up)
        Task<EntitySearchResult<AmphoraModel>> SearchAmphora(string searchText, SearchParameters parameters);
        Task<long?> SearchAmphoraCount(string searchText, SearchParameters parameters);
        Task<EntitySearchResult<T>> SearchAsync<T>(string searchText, SearchParameters parameters) where T : ISearchable;
        Task<bool> TryIndex();
    }
}