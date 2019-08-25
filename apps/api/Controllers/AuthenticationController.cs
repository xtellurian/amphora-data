using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Controllers
{

    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticateService authService;

        public AuthenticationController(IAuthenticateService authService)
        {
            this.authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("request")]
        public async Task<IActionResult> RequestToken([FromBody] TokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var token = await authService.IsAuthenticated(request);
            if (token.success)
            {
                return Ok(token.token);
            }

            return BadRequest("Invalid Request");
        }
    }
}