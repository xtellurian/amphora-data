using Amphora.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Controllers
{
    public abstract class EntityController : Controller
    {
        protected IActionResult Handle<T>(EntityOperationResult<T> result)
        {
            if (result.Succeeded)
            {
                return Ok();
            }

            if (result.Code == null)
            {
                if (result.WasForbidden)
                {
                    return StatusCode(403, result.Message);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return StatusCode(result.Code.Value, result.Message);
            }
        }
    }
}