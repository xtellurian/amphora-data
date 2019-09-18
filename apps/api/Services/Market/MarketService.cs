using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Api.Models.Search;
using Amphora.Common.Models;
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

        public async Task<IEnumerable<AmphoraModel>> FindAsync(string searchTerm)
        {
            if (searchTerm == null) searchTerm = "";
          
            var result = await searchService.SearchAmphora(searchTerm, SearchParameters.PublicAmphorae());

            return result.Results.Select(s => s.Entity);
        }
    }
}