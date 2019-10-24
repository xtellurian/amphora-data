using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Controllers.Amphorae
{
    public class PurchasesController: Controller
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
        /// <param name="id">Amphora Id</param>  
        [HttpPost("api/Amphorae/{id}/Purchases")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PurchaseAsync(string id)
        {
            var a = await amphoraeService.ReadAsync(User, id);
            if(a.Succeeded)
            {
                var result = await purchaseService.PurchaseAmphora(User, a.Entity);
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