using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace Amphora.Api.Controllers
{
    [SkipStatusCodePages]
    public class HealthzController : ControllerBase
    {
        private readonly ILogger<HealthzController> logger;

        public HealthzController(ILogger<HealthzController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Health endpoint.
        /// </summary>
        /// <returns> Simply 200. </returns>
        [HttpGet("healthz")]
        [OpenApiIgnore]
        public Task<IActionResult> HealthCheckAsync()
        {
            return Task<IActionResult>.Factory.StartNew(() =>
            {
                logger.LogDebug("Healthz ping");
                return Ok();
            });
        }
    }
}