using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Models;
namespace Amphora.Api.Services
{
    public class MarketService : IMarketService
    {
        private readonly IOrgScopedEntityStore<Common.Models.Amphora> amphoraStore;

        public MarketService(
            IOrgScopedEntityStore<Amphora.Common.Models.Amphora> amphoraStore)
        {
            this.amphoraStore = amphoraStore;
        }

        public async Task<IEnumerable<Amphora.Common.Models.Amphora>> FindAsync(string term, SearchParams searchParams)
        {
            var marketEntities = new List<Amphora.Common.Models.Amphora>();

            marketEntities.AddRange(await amphoraStore.ListAsync());


            if (string.IsNullOrEmpty(term)) return marketEntities;
            // string matching
            var results = marketEntities
                .Where(i =>
                   (i.Title?.ToLower()?.Contains(term?.ToLower()) ?? false)
                    ||
                   (i.Description?.ToLower()?.Contains(term?.ToLower()) ?? false)
                    );
            // price filter
            results = results.Where(i => searchParams.PriceFilter(i.Price));
            return results;
        }
    }
}