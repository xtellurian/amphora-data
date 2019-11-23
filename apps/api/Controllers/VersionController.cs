using Amphora.Api.AspNet;
using Microsoft.AspNetCore.Mvc;


namespace Amphora.Api.Controllers
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    public class VersionController : Controller
    {


        public VersionController()
        {
        }

        /// <summary>
        /// Get's the current server version
        /// </summary>
        [Produces(typeof(string))]
        [HttpGet("api/version/")]
        public IActionResult GetCurrentVersion()
        {
            return Ok(ApiVersion.CurrentVersion.ToSemver());
        }
    }
}
