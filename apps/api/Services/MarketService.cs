using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
namespace Amphora.Api.Services
{
    public class MarketService : IMarketService
    {
        private readonly IOrgEntityStore<Common.Models.Amphora> amphoraStore;
        private readonly IOrgEntityStore<Tempora> temporaStore;

        public MarketService(
            IOrgEntityStore<Amphora.Common.Models.Amphora> amphoraStore,
            IOrgEntityStore<Amphora.Common.Models.Tempora> temporaStore )
        {
            this.amphoraStore = amphoraStore;
            this.temporaStore = temporaStore;
        }

        public async Task<IEnumerable<Amphora.Common.Models.MarketEntity>> FindAsync(string term)
        {
            var marketEntities = new List<MarketEntity>();
            marketEntities.AddRange(await amphoraStore.ListAsync());
            marketEntities.AddRange(await temporaStore.ListAsync());
            if (string.IsNullOrEmpty(term)) return marketEntities;
            var result = marketEntities
                .Where( i =>
                    (i.Title?.ToLower()?.Contains(term?.ToLower()) ?? false)
                     ||
                    (i.Description?.ToLower()?.Contains(term?.ToLower()) ?? false)
                    );
            return result;
        }

        private IEnumerable<MarketEntity> FilterByTitle(IEnumerable<MarketEntity> input, string term)
        {
            return input.Where(i => i.Title?.ToLower()?.Contains(term?.ToLower()) ?? false);
        }

        private IEnumerable<MarketEntity> FilterByDescription(IEnumerable<MarketEntity> input, string term)
        {
            return input.Where(i => i.Description?.ToLower()?.Contains(term?.ToLower()) ?? false);
        }
    }
}