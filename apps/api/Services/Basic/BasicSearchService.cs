using System.Collections.Generic;
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

        // this just returns all the amphora is memory
        public async Task<EntitySearchResult<AmphoraModel>> SearchAmphora(string searchText, Models.Search.SearchParameters parameters)
        {
            var entities = new List<AmphoraModel>();

            var res = await amphoraeService.AmphoraStore.QueryAsync(
                a => a.Name.Contains(searchText) || a.Description.Contains(searchText));
            entities.AddRange(res);

            if (parameters.Skip.HasValue && parameters.Top.HasValue)
            {
                var toRemove = parameters.Skip * parameters.Top;
                entities.RemoveRange(0, toRemove.Value);
            }

            var take = parameters.Top ?? entities.Count;
            return new EntitySearchResult<AmphoraModel>(entities.Take(take).ToList());
        }

        public async Task<long?> SearchAmphoraCount(string searchText, SearchParameters parameters)
        {
            var res = await SearchAmphora(searchText, parameters);
            return res.Results.Count;
        }

        public Task<bool> TryIndex()
        {
            return Task.FromResult(true);
        }
    }
}