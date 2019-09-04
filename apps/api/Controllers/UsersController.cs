using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Api.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Amphora.Api.Controllers
{
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IUserService userService;
        private readonly IUserManager<ApplicationUser> userManager;
        private readonly IOptionsMonitor<CreateOptions> options;
        private readonly ILogger<UsersController> logger;

        public UsersController(IUserService userService,
                               IUserManager<ApplicationUser> userManager,
                               IOptionsMonitor<CreateOptions> options,
                               ILogger<UsersController> logger)
        {
            this.userService = userService;
            this.userManager = userManager;
            this.options = options;
            this.logger = logger;
        }

        [HttpPost("api/users")]
        public async Task<IActionResult> CreateUser_Key([FromBody] ApplicationUser user)
        {
            string password = System.Guid.NewGuid().ToString() + "!1A";
            if (Request.Headers.ContainsKey("Create"))
            {
                var value = Request.Headers["Create"];
                if (string.Equals(value, options.CurrentValue.Key))
                {
                    var result = await userService.CreateUserAsync(user, password);
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
                    var user = await userManager.FindByNameAsync(username);
                    if (user == null) return NotFound();
                    var result = await userManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        return Ok();
                    }
                    else
                    {
                        logger.LogError("Failed to create user!");
                        foreach (var e in result.Errors)
                        {
                            logger.LogError(e.Description);
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