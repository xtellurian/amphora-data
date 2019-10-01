using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Search;
using Amphora.Common.Models;
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

        // this just returns all the amphora is memory
        public async Task<EntitySearchResult<AmphoraModel>> SearchAmphora(string searchText, Models.Search.SearchParameters parameters)
        {
            var entities = new List<AmphoraModel>();

            var res = await amphoraeService.AmphoraStore.QueryAsync(
                a => a.Name.Contains(searchText)
                || (a.Description.Contains(searchText)));
            entities.AddRange(res);

            return new EntitySearchResult<AmphoraModel>(entities.ToList());
        }
    }
}