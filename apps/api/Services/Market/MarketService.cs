using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Api.Models.Search;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Services.Market
{
    public class MarketService : IMarketService
    {
        private readonly ISearchService searchService;

        public MarketService(
            ISearchService searchService)
        {
            this.searchService = searchService;
        }

        public async Task<IEnumerable<AmphoraModel>> FindAsync(string searchTerm)
        {
            if(searchTerm == null) return new List<AmphoraExtendedModel>();
            var p = new SearchParameters();
            var result = await searchService.SearchAmphora(searchTerm, p);

            return result.Results.Select(s => s.Entity);
        }
    }
}