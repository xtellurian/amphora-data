using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Search;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Amphora.Api.Services.Market
{
    public class MarketService : IMarketService, IDiscoverService<AmphoraModel>
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IUserService userService;
        private readonly ICache cache;

        public ISearchService SearchService { get; }

        public MarketService(
            ISearchService searchService,
            IAmphoraeService amphoraeService,
            IUserService userService,
            ICache cache)
        {
            this.SearchService = searchService;
            this.amphoraeService = amphoraeService;
            this.userService = userService;
            this.cache = cache;
        }

        public async Task<long?> CountAsync(string searchTerm,
                                            GeoLocation location = null,
                                            double? distance = null,
                                            int? skip = 0,
                                            int? top = 12,
                                            IEnumerable<string> labels = null)
        {
            searchTerm ??= "";
            var parameters = PrepareAmphoraParameters(location, distance, skip, top, labels);
            var key = searchTerm + JsonConvert.SerializeObject(parameters) + "count";
            long? count;
            if (!cache.TryGetValue(key, out count))
            {
                cache.Compact(50);
                count = await SearchService.SearchAmphoraCount(searchTerm, parameters);
                cache.Set(key, count, System.DateTime.Now.AddMinutes(5));
            }

            return count;
        }

        public async Task<EntitySearchResult<AmphoraModel>> FindAsync(string searchTerm,
                                                                      GeoLocation location = null,
                                                                      double? distance = null,
                                                                      int? skip = 0,
                                                                      int? top = 12,
                                                                      IEnumerable<string> labels = null)
        {
            searchTerm ??= "";
            var parameters = PrepareAmphoraParameters(location, distance, skip, top, labels);
            EntitySearchResult<AmphoraModel> result;
            var key = searchTerm + parameters.GetHashCode() + "search";
            if (!cache.TryGetValue(key, out result))
            {
                cache.Compact();
                result = await SearchService.SearchAmphora(searchTerm, parameters);
                cache.Set(key, result, System.DateTime.Now.AddMinutes(5));
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
                .PublicOnly<AmphoraModel>()
                .IncludeLabelsFacet<AmphoraModel>()
                .NotDeleted()
                .OrderByPurchaseCount<AmphoraModel>()
                .WithTotalResultCount();

            if (labels != null && labels.Count() > 0)
            {
                parameters = parameters.FilterByLabel<AmphoraModel>(new List<Label>(labels.Select(_ => new Label(_))));
            }

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