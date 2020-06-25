using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Models.Dtos.Geo;
using Amphora.Common.Contracts;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Amphora.Api.Controllers
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    [Route("api/geo")]
    [OpenApiTag("Geo")]
    public class GeoController : Controller
    {
        private readonly IMapService mapService;
        private readonly IMapper mapper;

        public GeoController(IMapService mapService, IMapper mapper)
        {
            this.mapService = mapService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Executes a fuzzy location search.
        /// </summary>
        /// <param name="query">Search Text.</param>
        /// <returns> A fuzzy search response. </returns>
        [Produces(typeof(FuzzySearchResponse))]
        [HttpGet("search/fuzzy")]
        [CommonAuthorize]
        public async Task<IActionResult> LookupLocationAsync(string query)
        {
            var response = await mapService.FuzzySearchAsync(query);
            var dto = mapper.Map<FuzzySearchResponse>(response);
            return Ok(dto);
        }
    }
}
