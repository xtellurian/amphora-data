using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Search;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Services.Basic
{
    public class BasicSearchService : ISearchService
    {
        private readonly IAmphoraeService amphoraeService;

        public BasicSearchService(
            IAmphoraeService amphoraeService)
        {
            this.amphoraeService = amphoraeService;
        }

        public async Task<EntitySearchResult<AmphoraModel>> SearchAmphora(string searchText, Models.Search.SearchParameters parameters)
        {
            var entities = await amphoraeService.AmphoraStore.QueryAsync<AmphoraExtendedModel>(
                a => a.Name.Contains(searchText)
                || (a.Description?.Contains(searchText) ?? false));

            return new EntitySearchResult<AmphoraModel>(entities.ToList());
        }
    }
}