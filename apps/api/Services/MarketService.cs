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
        private readonly IOrgScopedEntityStore<Tempora> temporaStore;

        public MarketService(
            IOrgScopedEntityStore<Amphora.Common.Models.Amphora> amphoraStore,
            IOrgScopedEntityStore<Amphora.Common.Models.Tempora> temporaStore )
        {
            this.amphoraStore = amphoraStore;
            this.temporaStore = temporaStore;
        }

        public async Task<IEnumerable<Amphora.Common.Models.MarketEntity>> FindAsync(string term, SearchParams searchParams)
        {
            var marketEntities = new List<MarketEntity>();
            if(searchParams?.IncludeAmphorae ?? true )
            {
                marketEntities.AddRange(await amphoraStore.ListAsync());
            }
            if(searchParams?.IncludeTemporae ?? true) 
            {
                marketEntities.AddRange(await temporaStore.ListAsync());
            }
            if (string.IsNullOrEmpty(term)) return marketEntities;
            // string matching
            var results = marketEntities
                .Where( i =>
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