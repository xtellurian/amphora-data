using System.Linq;
using Amphora.Api.AspNet;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Controllers
{
    [Route("identity")]
    [CommonAuthorize]
    public class IdentityController : ControllerBase
    {
        public IdentityController()
        {
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }
}