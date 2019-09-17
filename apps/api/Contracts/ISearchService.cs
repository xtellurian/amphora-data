using System.Threading.Tasks;
using Amphora.Api.Models.Search;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Contracts
{
    public interface ISearchService
    {
        Task<EntitySearchResult<AmphoraModel>> SearchAmphora(string searchText, SearchParameters parameters);
    }
}