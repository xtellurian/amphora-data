using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;
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
        private readonly ITransactionService transactionService;
        private readonly IAmphoraeService amphoraeService;

        public MarketController(IMarketService marketService, IMapService mapService,
                                ITransactionService transactionService, IAmphoraeService amphoraeService)
        {
            this.marketService = marketService;
            this.mapService = mapService;
            this.transactionService = transactionService;
            this.amphoraeService = amphoraeService;
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

        [HttpPost("api/market/purchase/")]
        public async Task<IActionResult> PurchaseAsync(string id)
        {
            var a = await amphoraeService.ReadAsync<AmphoraModel>(User, id);
            if(a.Succeeded)
            {
                var result = await transactionService.PurchaseAmphora(User, a.Entity);
                if(result.Succeeded)
                {
                    return Ok();
                }
                else if(result.WasForbidden)
                {
                    return StatusCode(403);
                }
                else
                {
                    return BadRequest();
                }
            }
            else if(a.WasForbidden)
            {
                return StatusCode(403);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
