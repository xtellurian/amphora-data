using System.Threading.Tasks;
using Amphora.Api.Contracts;
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

        [HttpGet("api/market")]
        public async Task<IActionResult> FindAsync(string query)
        {
            var s = await marketService.FindAsync(query);
            return Ok(s);
        }
    }
}
