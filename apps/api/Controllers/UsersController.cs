using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Api.Options;
using Amphora.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Amphora.Api.Controllers
{
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IOnboardingService onboardingService;
        private readonly IUserService userService;
        private readonly IOptionsMonitor<CreateOptions> options;
        private readonly ILogger<UsersController> logger;

        public UsersController(IOnboardingService onboardingService,
                               IUserManager userManager,
                               IUserService userService,
                               IOptionsMonitor<CreateOptions> options,
                               ILogger<UsersController> logger)
        {
            this.onboardingService = onboardingService;
            this.userService = userService;
            this.options = options;
            this.logger = logger;
        }

        [HttpGet("api/users/self")]
        public async Task<IActionResult> ReadSelf()
        {
            var user = await userService.UserManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound();
            }
            return Ok(user as IApplicationUser);
        }

        [HttpPost("api/users")]
        public async Task<IActionResult> CreateUser([FromBody] ApplicationUser user, string onboardingId)
        {
            string password = System.Guid.NewGuid().ToString() + "!1A";
            if (Request.Headers.ContainsKey("Create"))
            {
                var value = Request.Headers["Create"];
                if (string.Equals(value, options.CurrentValue.Key))
                {
                    var result = await onboardingService.CreateUserAsync(user, password, onboardingId);
                    if (result.Succeeded)
                    {
                        return Ok(password);
                    }
                    else
                    {
                        logger.LogError("Failed to create user!");
                        foreach (var e in result.Errors)
                        {
                            logger.LogError(e);
                        }
                    }
                    return BadRequest(JsonConvert.SerializeObject(result.Errors));
                }
                else
                {
                    return Unauthorized("Incorrect Create Key");
                }
            }
            else
            {
                return Unauthorized("Create Header Required");
            }
        }

        [HttpDelete("api/users/{username}")]
        public async Task<IActionResult> DeleteUser_Key(string username)
        {
            if (Request.Headers.ContainsKey("Create"))
            {
                var value = Request.Headers["Create"];
                if (string.Equals(value, options.CurrentValue.Key))
                {
                    if (username == null) return BadRequest("id/ username is required");
                    var user = await userService.UserManager.FindByNameAsync(username);
                    if (user == null) return NotFound();
                    var result = await userService.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        return Ok();
                    }
                    else
                    {
                        logger.LogError("Failed to create user!");
                        foreach (var e in result.Errors)
                        {
                            logger.LogError(e);
                        }
                    }
                    return BadRequest(JsonConvert.SerializeObject(result.Errors));
                }
                else
                {
                    return Unauthorized("Incorrect Create Key");
                }
            }
            else
            {
                return Unauthorized("Create Header Required");
            }
        }
    }
}