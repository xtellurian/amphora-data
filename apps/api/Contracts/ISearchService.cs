using System.Threading.Tasks;
using Amphora.Api.Models.Search;
using Amphora.Common.Contracts;

namespace Amphora.Api.Contracts
{
    public interface ISearchService
    {
        Task<EntitySearchResult<T>> SearchAsync<T>(string searchText, SearchParameters parameters) where T : ISearchable;
        Task<bool> TryIndex();
    }
}