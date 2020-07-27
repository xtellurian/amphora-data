using System.Threading.Tasks;
using Amphora.Api.Models.Search;
using Amphora.Common.Contracts;

namespace Amphora.Api.Contracts
{
    public interface ISearchService
    {
        Task<long?> SearchAmphoraCount(string searchText, SearchParameters parameters);
        Task<EntitySearchResult<T>> SearchAsync<T>(string searchText, SearchParameters parameters = null) where T : ISearchable;
        Task<bool> TryIndex();
    }
}