using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        [HttpGet("healthz")]
        public Task<IActionResult> HealthCheckAsync()
        {
            return Task<IActionResult>.Factory.StartNew(() =>
            {
                logger.LogInformation("Healthz ping");
                return Ok();
            });
        }
    }
}