using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Contracts;
using Amphora.Common.Models.AzureMaps;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Controllers
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    public class MarketController : Controller
    {
        private readonly IMarketService marketService;
        private readonly IMapService mapService;
        private readonly IMapper mapper;
        private readonly IPurchaseService purchaseService;
        private readonly IAmphoraeService amphoraeService;

        public MarketController(IMarketService marketService, IMapService mapService, IMapper mapper,
                                IPurchaseService purchaseService, IAmphoraeService amphoraeService)
        {
            this.marketService = marketService;
            this.mapService = mapService;
            this.mapper = mapper;
            this.purchaseService = purchaseService;
            this.amphoraeService = amphoraeService;
        }

        /// <summary>
        /// Executes a fuzzy location search.
        /// </summary>
        /// <param name="query">Search Text.</param>
        /// <returns> A fuzzy search response. </returns>
        [Produces(typeof(FuzzySearchResponse))]
        [HttpGet("api/location/fuzzy")]
        [CommonAuthorize]
        public async Task<IActionResult> LookupLocationAsync(string query)
        {
            var response = await mapService.FuzzySearchAsync(query);
            return Ok(response);
        }

        /// <summary>
        /// Finds Amphora using a fuzzy search.
        /// </summary>
        /// <param name="query">A string as a search term.</param>
        /// <param name="top">How many results to return.</param>
        /// <param name="skip">How many pages (in multiples of top) to skip.</param>
        /// <returns> A collection of Amphora. </returns>
        [Produces(typeof(List<BasicAmphora>))]
        [HttpGet("api/market/search")]
        [CommonAuthorize]
        public async Task<IActionResult> Find(string query, int? top, int? skip)
        {
            var searchResult = await marketService.FindAsync(query, skip: skip, top: top);
            var entities = searchResult.Results.Select(_ => _.Entity);
            var dto = mapper.Map<List<BasicAmphora>>(entities);
            return Ok(dto);
        }
    }
}
