using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Contracts
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthorizationsController: Controller
    {
        public AuthorizationsController()
        {
            
        }
        [HttpPost("api/authoriations")]
        public Task<IActionResult> CreateResourceAuthorization()
        {
            throw new NotImplementedException();
        }
    }
}