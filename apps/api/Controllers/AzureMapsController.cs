using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Common.Contracts;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Amphora.Api.Controllers
{
    [Route("api/maps")]
    [OpenApiIgnore]
    [CommonAuthorize]
    public class AzureMapsController : ControllerBase
    {
        private readonly IMapService mapService;

        public AzureMapsController(IMapService mapService)
        {
            this.mapService = mapService;
        }

        [HttpGet("key")]
        public async Task<IActionResult> GetToken()
        {
            var token = await mapService.GetSubscriptionKeyAsync();
            return Ok(token);
        }
    }
}