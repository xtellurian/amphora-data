using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Api.Models.Dtos.Search;
using Amphora.Api.Models.Search;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace Amphora.Api.Controllers
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    public class SearchController : Controller
    {
        private readonly IMapper mapper;
        private readonly ISearchService searchService;
        private readonly ILogger<SearchController> logger;

        public SearchController(IMapper mapper,
                                ISearchService searchService,
                                IUserDataService userDataService,
                                ILogger<SearchController> logger)
        {
            this.mapper = mapper;
            this.searchService = searchService;
            this.logger = logger;
        }

        /// <summary>
        /// Searches for Amphorae.
        /// </summary>
        /// <returns> A collection of Amphora. </returns>
        [Produces(typeof(List<BasicAmphora>))]
        [HttpGet("api/search/amphorae")]
        [CommonAuthorize]
        public async Task<IActionResult> SearchAmphorae([FromQuery] AmphoraSearchParameters queryParameters)
        {
            var parameters = new SearchParameters();
            parameters.Skip = queryParameters.Skip;
            parameters.Top = queryParameters.Take;
            var labelsArray = queryParameters.Labels?.Split(',')?.ToList();
            if (labelsArray != null && labelsArray.Count > 0)
            {
                parameters = parameters.FilterByLabel<AmphoraModel>(new List<Label>(labelsArray.Select(_ => new Label(_))));
            }

            if (queryParameters.Lat != null && queryParameters.Lon != null)
            {
                parameters.WithGeoSearch<AmphoraModel>(queryParameters.Lat.Value, queryParameters.Lon.Value, queryParameters.Dist ?? 50);
            }

            if (!string.IsNullOrEmpty(queryParameters.OrgId))
            {
                parameters = parameters.FilterByOrganisation<AmphoraModel>(queryParameters.OrgId);
            }

            try
            {
                var response = await searchService.SearchAmphora(queryParameters.Term ?? "", parameters);
                var entities = response.Results.Select(a => a.Entity);
                var dto = mapper.Map<List<BasicAmphora>>(entities);
                return Ok(dto);
            }
            catch (System.Exception ex)
            {
                logger.LogError("Search API failed", ex);
                return BadRequest("Something went wrong with the search.");
            }
        }

        /// <summary>
        /// Searches for Amphorae by loction.
        /// </summary>
        /// <param name="lat">Latitude.</param>
        /// <param name="lon">Longitude.</param>
        /// <param name="dist">Distance from Latitude and Longitude in which to search.</param>
        /// <returns>A collection of Amphora.</returns>
        [OpenApiIgnore]
        [HttpGet("api/search/amphorae/byLocation")]
        public IActionResult SearchAmphoraeByLocation(double lat, double lon, double dist = 10)
        {
            return Redirect($"api/search/amphorae?lat={lat}$lon={lon}&dist={dist}");
        }

        /// <summary>
        /// Searches for Amphorae in an Organisation.
        /// </summary>
        /// <param name="orgId">Organisation Id.</param>
        /// <returns> A collection of Amphora. </returns>
        [OpenApiIgnore]
        [HttpGet("api/search/amphorae/byOrganisation")]
        public IActionResult SearchAmphoraeByOrganisation(string orgId)
        {
            return Redirect($"api/search/amphorae?orgId={orgId}");
        }

        /// <summary>
        /// Searches for Organisations with fuzzy search.
        /// </summary>
        /// <param name="term">Search Term.</param>
        /// <returns> A collection of Organisations. </returns>
        [Produces(typeof(List<Organisation>))]
        [HttpGet("api/search/organisations/")]
        [CommonAuthorize]
        public async Task<IActionResult> SearchOrganisations(string term)
        {
            var res = await searchService.SearchAsync<OrganisationModel>(term);
            var dto = mapper.Map<List<Organisation>>(res.Results.Select(_ => _.Entity));
            return Ok(dto);
        }

        [HttpPost("api/search/indexers")]
        [OpenApiIgnore]
        [CommonAuthorize]
        public async Task<IActionResult> TryReindex()
        {
            var res = await searchService.TryIndex();
            if (res) { return Ok(); }
            else { return BadRequest(); }
        }
    }
}
