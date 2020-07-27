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
using NSwag.Annotations;

namespace Amphora.Api.Controllers
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    public class MarketController : Controller
    {
        private readonly IMapService mapService;
        private readonly IMapper mapper;
        private readonly IPurchaseService purchaseService;
        private readonly IAmphoraeService amphoraeService;

        public MarketController(IMapService mapService, IMapper mapper,
                                IPurchaseService purchaseService, IAmphoraeService amphoraeService)
        {
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
        [OpenApiIgnore] // ignore in favour of the GeoController
        [HttpGet("api/location/fuzzy")]
        [CommonAuthorize]
        public async Task<IActionResult> LookupLocationAsync(string query)
        {
            var response = await mapService.FuzzySearchAsync(query);
            return Ok(response);
        }
    }
}
