using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Api.Models.Search;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Users;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

        public SearchController(IMapper mapper, ISearchService searchService, IUserDataService userDataService)
        {
            this.mapper = mapper;
            this.searchService = searchService;
        }

        /// <summary>
        /// Searches for Amphorae.
        /// </summary>
        /// <param name="parameters">Search parameters.</param>
        /// <returns> A collection of Amphora. </returns>
        [Produces(typeof(List<BasicAmphora>))]
        [HttpPost("api/search/amphorae")]
        [CommonAuthorize]
        [OpenApiIgnore]
        public async Task<IActionResult> SearchAmphorae([FromBody] SearchParameters parameters)
        {
            var response = await searchService.SearchAmphora("", parameters);
            var entities = response.Results.Select(a => a.Entity);
            var dto = mapper.Map<List<BasicAmphora>>(entities);
            return Ok(dto);
        }

        /// <summary>
        /// Searches for Amphorae by loction.
        /// </summary>
        /// <param name="lat">Latitude.</param>
        /// <param name="lon">Longitude.</param>
        /// <param name="dist">Distance from Latitude and Longitude in which to search.</param>
        /// <returns>A collection of Amphora.</returns>
        [Produces(typeof(List<BasicAmphora>))]
        [HttpGet("api/search/amphorae/byLocation")]
        [CommonAuthorize]
        public async Task<IActionResult> SearchAmphoraeByLocation(double lat, double lon, double dist = 10)
        {
            var response = await searchService.SearchAmphora("", new SearchParameters().WithGeoSearch<AmphoraModel>(lat, lon, dist));
            var entities = response.Results.Select(a => a.Entity);
            var dto = mapper.Map<List<BasicAmphora>>(entities);
            return Ok(dto);
        }

        /// <summary>
        /// Searches for Amphorae in an Organisation.
        /// </summary>
        /// <param name="orgId">Organisation Id.</param>
        /// <returns> A collection of Amphora. </returns>
        [Produces(typeof(List<BasicAmphora>))]
        [HttpGet("api/search/amphorae/byOrganisation")]
        [CommonAuthorize]
        public async Task<IActionResult> SearchAmphoraeByOrganisation(string orgId)
        {
            if (string.IsNullOrEmpty(orgId)) { return BadRequest("OrgId cannot be null"); }
            var response = await searchService.SearchAmphora("", new SearchParameters().FilterByOrganisation<AmphoraModel>(orgId));
            var entities = response.Results.Select(a => a.Entity);
            var dto = mapper.Map<List<BasicAmphora>>(entities);
            return Ok(dto);
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
