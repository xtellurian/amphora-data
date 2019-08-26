using System.Diagnostics;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Amphora.Api.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MarketController : Controller
    {
        private readonly IMarketService marketService;
        private readonly IMapService mapService;

        public MarketController(IMarketService marketService, IMapService mapService)
        {
            this.marketService = marketService;
            this.mapService = mapService;
        }

        [HttpGet("api/location/fuzzy")]
        public async Task<IActionResult> LookupLocationAsync(string query)
        {
            var response = await mapService.FuzzySearchAsync(query);
            return Ok(response);
        }

        [HttpPost("api/market")]
        public async Task<IActionResult> FindAsync([FromBody] SearchParams p)
        {
            var s = await marketService.FindAsync(p);
            return Ok(s);
        }
    }
}
