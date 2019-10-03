using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Api.Models.Search;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Users;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Amphora.Api.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        [HttpPost("api/search/amphorae")]
        public async Task<IActionResult> SearchAmphorae([FromBody] SearchParameters parameters)
        {
            var response = await searchService.SearchAmphora("", parameters);
            var entities = response.Results.Select(a => a.Entity);
            var dto = mapper.Map<List<AmphoraDto>>(entities);
            return Ok(dto);
        }

        [HttpGet("api/search/amphorae/byLocation")]
        public async Task<IActionResult> SearchAmphoraeByLocation(double lat, double lon, double dist = 10)
        {
            var response = await searchService.SearchAmphora("", SearchParameters.GeoSearch(lat, lon, dist));
            var entities = response.Results.Select(a => a.Entity);
            var dto = mapper.Map<List<AmphoraDto>>(entities);
            return Ok(dto);
        }
        [HttpGet("api/search/amphorae/byOrganisation")]
        public async Task<IActionResult> SearchAmphoraeByOrganisation(string orgId)
        {
            if (string.IsNullOrEmpty(orgId)) return BadRequest("OrgId cannot be null");
            var response = await searchService.SearchAmphora("", SearchParameters.ByOrganisation(orgId, typeof(AmphoraModel)));
            var entities = response.Results.Select(a => a.Entity);
            var dto = mapper.Map<List<AmphoraDto>>(entities);
            return Ok(dto);
        }

        [HttpGet("api/search/amphorae/byCreator")]
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
    }
}
