using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Models.Search;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Users;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IUserService userService;

        public SearchController(IMapper mapper, ISearchService searchService, IUserService userService)
        {
            this.mapper = mapper;
            this.searchService = searchService;
            this.userService = userService;
        }

        /// <summary>
        /// Searches for Amphorae.
        /// </summary>
        /// <param name="parameters">Search parameters</param>
        [Produces(typeof(List<AmphoraDto>))]
        [HttpPost("api/search/amphorae")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SearchAmphorae([FromBody] SearchParameters parameters)
        {
            var response = await searchService.SearchAmphora("", parameters);
            var entities = response.Results.Select(a => a.Entity);
            var dto = mapper.Map<List<AmphoraDto>>(entities);
            return Ok(dto);
        }
        /// <summary>
        /// Searches for Amphorae by loction.
        /// </summary>
        /// <param name="lat">Latitude</param>
        /// <param name="lon">Longitude</param>
        /// <param name="dist">Distance from Latitude and Longitude in which to search</param>
        [Produces(typeof(List<AmphoraDto>))]
        [HttpGet("api/search/amphorae/byLocation")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SearchAmphoraeByLocation(double lat, double lon, double dist = 10)
        {
            var response = await searchService.SearchAmphora("", SearchParameters.GeoSearch(lat, lon, dist));
            var entities = response.Results.Select(a => a.Entity);
            var dto = mapper.Map<List<AmphoraDto>>(entities);
            return Ok(dto);
        }

        /// <summary>
        /// Searches for Amphorae in an Organisation.
        /// </summary>
        /// <param name="orgId">Organisation Id</param>
        [Produces(typeof(List<AmphoraDto>))]
        [HttpGet("api/search/amphorae/byOrganisation")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SearchAmphoraeByOrganisation(string orgId)
        {
            if (string.IsNullOrEmpty(orgId)) return BadRequest("OrgId cannot be null");
            var response = await searchService.SearchAmphora("", SearchParameters.ByOrganisation(orgId, typeof(AmphoraModel)));
            var entities = response.Results.Select(a => a.Entity);
            var dto = mapper.Map<List<AmphoraDto>>(entities);
            return Ok(dto);
        }
        /// <summary>
        /// Searches for Amphorae by creator.
        /// </summary>
        /// <param name="userName">User Name of the creator</param>
        [Produces(typeof(List<AmphoraDto>))]
        [HttpGet("api/search/amphorae/byCreator")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SearchAmphoraeByCreator(string userName)
        {
            ApplicationUser user;
            if (string.IsNullOrEmpty(userName))
            {
                user = await userService.ReadUserModelAsync(User);
            }
            else
            {
                user = (await userService.UserManager.FindByNameAsync(userName));
            }

            var response = await searchService.SearchAmphora("", SearchParameters.ForUserAsCreator(user));
            var entities = response.Results.Select(a => a.Entity);
            var dto = mapper.Map<List<AmphoraDto>>(entities);
            return Ok(dto);
        }

        [HttpPost("api/search/indexers")]
        [OpenApiIgnore]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> TryReindex()
        {
            var res = await searchService.TryIndex();
            if (res) return Ok();
            else return BadRequest();
        }
    }
}
