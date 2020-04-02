using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amphora.Common.Models.Platform;
using Amphora.Identity.Models;
using IdentityServer4;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Amphora.Identity.Controllers
{
    public class TokenController : Controller
    {
        private readonly IdentityServerTools tools;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> claimsPrincipalFactory;
        private readonly ILogger<TokenController> logger;

        public TokenController(IdentityServerTools tools,
                               SignInManager<ApplicationUser> signInManager,
                               UserManager<ApplicationUser> userManager,
                               IUserClaimsPrincipalFactory<ApplicationUser> claimsPrincipalFactory,
                               ILogger<TokenController> logger)
        {
            this.tools = tools;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.claimsPrincipalFactory = claimsPrincipalFactory;
            this.logger = logger;
        }

        [HttpPost("api/token")]
        public async Task<IActionResult> RequestToken([FromBody] LoginRequest request)
        {
            var result = await signInManager.PasswordSignInAsync(request.Username, request.Password, false, true);

            if (result.Succeeded)
            {
                var user = await userManager.FindByNameAsync(request.Username);
                logger.LogInformation($"Issuing Token for {request.Username}");
                var claimsPrincipal = await claimsPrincipalFactory.CreateAsync(user);
                var message = new StringBuilder();
                foreach (var c in claimsPrincipal.Claims)
                {
                    message.AppendLine($"Claim {c.Type} : {c.Value}");
                }

                logger.LogInformation($"User {request.Username} has {claimsPrincipal.Claims.Count()} claims");
                logger.LogInformation(message.ToString());
                // issue the token - THIS MIGHT NOT WORK, OTHER METHOD MIGHT BE BETTER
                var token = await tools.IssueJwtAsync(lifetime: 3600, claims: claimsPrincipal.Claims);
                return Ok(token);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}