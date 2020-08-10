using Amphora.Api.Models.Dtos;
using Amphora.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Controllers
{
    public abstract class EntityController : Controller
    {
        protected IActionResult Handle<T>(EntityOperationResult<T> result) where T : class
        {
            if (result.Succeeded)
            {
                return Ok(new Response("Success"));
            }

            if (result.Code == null)
            {
                if (result.WasForbidden)
                {
                    return StatusCode(403, new Response(result.Message));
                }
                else
                {
                    return BadRequest(new Response(result.Message));
                }
            }
            else
            {
                return StatusCode(result.Code.Value, new Response(result.Message));
            }
        }
    }
}