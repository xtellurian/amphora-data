using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.AzureMaps;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Amphora.Api.Controllers
{
    [ApiController]
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
        /// <param name="query">Search Text</param>
        [Produces(typeof(FuzzySearchResponse))]
        [HttpGet("api/location/fuzzy")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> LookupLocationAsync(string query)
        {
            var response = await mapService.FuzzySearchAsync(query);
            return Ok(response);
        }

        /// <summary>
        /// Finds Amphora using a fuzzy search
        /// </summary>
        /// <param name="query">Amphora Id</param>
        [Produces(typeof(List<AmphoraDto>))]
        [HttpGet("api/market")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> FindAsync(string query)
        {
            var s = await marketService.FindAsync(query);
            var dto = mapper.Map<List<AmphoraDto>>(s);
            return Ok(dto);
        }
    }
}
