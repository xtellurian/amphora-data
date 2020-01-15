using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Search;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.DataRequests;
using Microsoft.Extensions.Caching.Memory;

namespace Amphora.Api.Services.Discover
{
    public class DataRequestDiscoveryService : IDiscoverService<DataRequestModel>
    {
        private readonly ISearchService searchService;
        private readonly IMemoryCache cache;

        public DataRequestDiscoveryService(ISearchService searchService,
                                           IMemoryCache cache)
        {
            this.searchService = searchService;
            this.cache = cache;
        }

        public async Task<EntitySearchResult<DataRequestModel>> FindAsync(string searchTerm,
                                                                    GeoLocation location = null,
                                                                    double? distance = null,
                                                                    int? skip = 0,
                                                                    int? top = 15,
                                                                    IEnumerable<string> labels = null)
        {
            searchTerm ??= "";
            var parameters = PrepareAmphoraParameters(location, distance, skip, top, labels);
            EntitySearchResult<DataRequestModel> result;
            var key = searchTerm + parameters.GetHashCode() + "search";
            if (!cache.TryGetValue(key, out result))
            {
                result = await searchService.SearchAsync<DataRequestModel>(searchTerm, parameters);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(System.DateTime.Now.AddMinutes(5));
                cache.Set(key, result, cacheEntryOptions);
            }

            return result;
        }

        private static SearchParameters PrepareAmphoraParameters(GeoLocation location,
                                                          double? distance,
                                                          int? skip,
                                                          int? top,
                                                          IEnumerable<string> labels)
        {
            var d = distance.HasValue ? distance.Value : 100; // default to 100km
            var parameters = new SearchParameters()
                .NotDeleted()
                .WithTotalResultCount();

            if (location != null && location.Lat().HasValue && location.Lon().HasValue)
            {
                parameters.WithGeoSearch<AmphoraModel>(location.Lat().Value, location.Lon().Value, d);
            }

            parameters.Top = top;
            parameters.Skip = skip;

            return parameters;
        }
    }
}
