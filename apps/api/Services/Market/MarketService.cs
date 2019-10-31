using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Search;
using Amphora.Common.Models.Amphorae;
using AutoMapper;

namespace Amphora.Api.Services.Market
{
    public class MarketService : IMarketService
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IMapper mapper;
        private readonly IUserService userService;

        public ISearchService searchService { get; }

        public MarketService(
            ISearchService searchService,
            IAmphoraeService amphoraeService,
            IMapper mapper,
            IUserService userService)
        {
            this.searchService = searchService;
            this.amphoraeService = amphoraeService;
            this.mapper = mapper;
            this.userService = userService;
        }

        public async Task<IEnumerable<AmphoraModel>> FindAsync(string searchTerm, GeoLocation location = null, double? distance = null, int? skip = 0, int? top = 12)
        {
            if (searchTerm == null) searchTerm = "";
            var d = distance.HasValue ? distance.Value : 100; // default to 100km
            var parameters = new SearchParameters().WithPublicAmphorae();
            if(location != null && location.Lat().HasValue && location.Lon().HasValue)
            {
                parameters.WithGeoSearch(location.Lat().Value, location.Lon().Value, d);
            }
            parameters.Top = top;
            parameters.Skip = skip;

            var result = await searchService.SearchAmphora(searchTerm, parameters);

            return result.Results.Select(s => s.Entity);
        }
    }
}   