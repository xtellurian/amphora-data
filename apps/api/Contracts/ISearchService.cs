namespace Amphora.Api.Contracts
{
    public interface ISearchService
    {
        System.Threading.Tasks.Task<Models.Search.EntitySearchResult<Common.Models.AmphoraModel>> SearchAmphora(string searchText, Models.Search.SearchParameters parameters);
    }
}