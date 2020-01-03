using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Search;
using Amphora.Common.Models.Amphorae;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Amphora.Api.Services.Market
{
    public class MarketService : IMarketService
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IMapper mapper;
        private readonly IUserService userService;
        private readonly IMemoryCache cache;

        public ISearchService searchService { get; }

        public MarketService(
            ISearchService searchService,
            IAmphoraeService amphoraeService,
            IMapper mapper,
            IUserService userService,
            IMemoryCache cache)
        {
            this.searchService = searchService;
            this.amphoraeService = amphoraeService;
            this.mapper = mapper;
            this.userService = userService;
            this.cache = cache;
        }

        public async Task<long?> CountAsync(string searchTerm, GeoLocation location = null, double? distance = null, int? skip = 0, int? top = 12, IEnumerable<string> labels = null)
        {
            searchTerm ??= "";
            var parameters = PrepareParameters(location, distance, skip, top, labels);
            var key = searchTerm + JsonConvert.SerializeObject(parameters) + "count";
            long? count;
            if (!cache.TryGetValue(key, out count))
            {
                count = await searchService.SearchAmphoraCount(searchTerm, parameters);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(System.DateTime.Now.AddMinutes(5));
                cache.Set(key, count, cacheEntryOptions);
            }

            return count;
        }

        public async Task<EntitySearchResult<AmphoraModel>> FindAsync(string searchTerm, GeoLocation location = null, double? distance = null, int? skip = 0, int? top = 12, IEnumerable<string> labels = null)
        {
            searchTerm ??= "";
            var parameters = PrepareParameters(location, distance, skip, top, labels);
            EntitySearchResult<AmphoraModel> result;
            var key = searchTerm + parameters.GetHashCode() + "search";
            if (!cache.TryGetValue(key, out result))
            {
                result = await searchService.SearchAmphora(searchTerm, parameters);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(System.DateTime.Now.AddMinutes(5));
                cache.Set(key, result, cacheEntryOptions);
            }

            return result;
        }

        private static SearchParameters PrepareParameters(GeoLocation location, double? distance, int? skip, int? top, IEnumerable<string> labels)
        {
            var d = distance.HasValue ? distance.Value : 100; // default to 100km
            var parameters = new SearchParameters().WithPublicAmphorae().NotDeleted().IncludeLabelsFacet();
            if (labels != null && labels.Count() > 0)
            {
                parameters = parameters.FilterByLabel(new List<Label>(labels.Select(_ => new Label(_))));
            }
            if (location != null && location.Lat().HasValue && location.Lon().HasValue)
            {
                parameters.WithGeoSearch(location.Lat().Value, location.Lon().Value, d);
            }
            parameters.Top = top;
            parameters.Skip = skip;

            return parameters;
        }
    }
}