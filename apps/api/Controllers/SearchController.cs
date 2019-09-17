using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Search;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Amphora.Api.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SearchController : Controller
    {
        private readonly ISearchService searchService;
        private readonly IUserService userService;

        public SearchController(ISearchService searchService, IUserService userService)
        {
            this.searchService = searchService;
            this.userService = userService;
        }

        [HttpGet("api/search/amphorae")]
        public async Task<IActionResult> LookupLocationAsync(SearchParameters parameters)
        {
            var response = await searchService.SearchAmphora("", parameters);
            return Ok(response.Results.Select(a => a.Entity));
        }
    }
}
