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

        public Task Reindex()
        {
            return Task.CompletedTask;
        }

        public async Task<EntitySearchResult<AmphoraModel>> SearchAmphora(string searchText, Models.Search.SearchParameters parameters)
        {
            var entities = new List<AmphoraModel>();
            if(parameters.Filter?.StartsWith($"{nameof(Entity.CreatedBy)} eq") ?? false)
            {
                var userId = parameters.Filter.Split(' ')[2];
                var res = await amphoraeService.AmphoraStore.QueryAsync<AmphoraExtendedModel>(
                    a => 
                    string.Equals(a.CreatedBy, userId) &&
                    (a.Name.Contains(searchText)
                    || (a.Description?.Contains(searchText) ?? false)));
                entities.AddRange(res);
            }
            else
            {
                var res = await amphoraeService.AmphoraStore.QueryAsync<AmphoraExtendedModel>(
                    a => a.Name.Contains(searchText)
                    || (a.Description?.Contains(searchText) ?? false));
                entities.AddRange(res);
            }

            return new EntitySearchResult<AmphoraModel>(entities.ToList());
        }
    }
}