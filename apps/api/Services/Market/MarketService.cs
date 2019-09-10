using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;

namespace Amphora.Api.Services.Market
{
    public class MarketService : IMarketService
    {
        private readonly IAmphoraeService amphoraeService;

        public MarketService(
            IAmphoraeService amphoraeService)
        {
            this.amphoraeService = amphoraeService;
        }

        public async Task<IEnumerable<Amphora.Common.Models.Amphora>> FindAsync(SearchParams searchParams)
        {
            IList<Amphora.Common.Models.Amphora> entities;
            if(searchParams == null) return new List<Amphora.Common.Models.Amphora>();
            
            if(searchParams.IsGeoSearch)
            {
                entities = await amphoraeService.AmphoraStore.StartsWithQueryAsync(
                    nameof(Amphora.Common.Models.Amphora.GeoHash), 
                    searchParams.GeoHashStartsWith);
            }
            else
            {
                entities = await amphoraeService.AmphoraStore.ListAsync();
            }
            
            // string matching
            if(string.IsNullOrEmpty(searchParams.SearchTerm)) return entities;
            var results = entities
                .Where(i =>
                   (i.Title?.ToLower()?.Contains(searchParams?.SearchTerm?.ToLower()) ?? false)
                    ||
                   (i.Description?.ToLower()?.Contains(searchParams.SearchTerm?.ToLower()) ?? false)
                    );
            // price filter
            results = results.Where(i => searchParams.PriceFilter(i.Price));
            return results;
        }
    }
}