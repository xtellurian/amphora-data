using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Amphora.Api.Controllers.Amphorae
{
    [ApiMajorVersion(0)]
    [SkipStatusCodePages]
    [OpenApiTag("Amphorae")]
    [Route("api/Amphorae/{id}/Purchases")]
    public class PurchasesController : Controller
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IPurchaseService purchaseService;

        public PurchasesController(IAmphoraeService amphoraeService, IPurchaseService purchaseService)
        {
            this.amphoraeService = amphoraeService;
            this.purchaseService = purchaseService;
        }

        /// <summary>
        /// Purchases an Amphora as the logged in user.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <returns>A Message.</returns>
        [HttpPost]
        [Produces(typeof(string))]
        [CommonAuthorize]
        public async Task<IActionResult> Purchase(string id)
        {
            var a = await amphoraeService.ReadAsync(User, id);
            if (a.Succeeded)
            {
                if (!await purchaseService.CanPurchaseAmphoraAsync(User, a.Entity))
                {
                    return StatusCode(403);
                }

                var result = await purchaseService.PurchaseAmphoraAsync(User, a.Entity);
                if (result.Succeeded)
                {
                    return Ok("Purchased Amphora");
                }
                else if (result.WasForbidden)
                {
                    return StatusCode(403);
                }
                else
                {
                    return BadRequest();
                }
            }
            else if (a.WasForbidden)
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