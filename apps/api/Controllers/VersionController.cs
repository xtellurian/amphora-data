using Amphora.Api.AspNet;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Amphora.Api.Controllers
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    [OpenApiTag("Version")]
    public class VersionController : Controller
    {
        public VersionController()
        {
        }

        /// <summary>
        /// Gets the current server version.
        /// </summary>
        /// <returns> The current version string.</returns>
        [Produces(typeof(string))]
        [HttpGet("api/version/")]
        public IActionResult GetCurrentVersion()
        {
            return Ok(ApiVersion.CurrentVersion.ToSemver());
        }
    }
}
