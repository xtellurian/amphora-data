using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos;
using Amphora.Common.Security;
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
        /// <returns>A Response with a message.</returns>
        [HttpPost]
        [Produces(typeof(Response))]
        [ProducesDefaultResponseType(typeof(Response))]
        [CommonAuthorize(Policies.RequirePurchaseClaim)]
        public async Task<IActionResult> Purchase(string id)
        {
            var a = await amphoraeService.ReadAsync(User, id);
            if (a.Succeeded)
            {
                if (!await purchaseService.CanPurchaseAmphoraAsync(User, a.Entity))
                {
                    return StatusCode(403, new Response("You are not allowed to purchase this Amphora"));
                }

                var result = await purchaseService.PurchaseAmphoraAsync(User, a.Entity);
                if (result.Succeeded)
                {
                    return Ok(new Response("Purchased Amphora"));
                }
                else if (result.WasForbidden)
                {
                    return StatusCode(403, new Response(result.Message));
                }
                else
                {
                    return BadRequest(new Response(result.Message));
                }
            }
            else if (a.WasForbidden)
            {
                return StatusCode(403, new Response(a.Message));
            }
            else
            {
                return NotFound(new Response(a.Message));
            }
        }
    }
}