using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Users;
using Amphora.Api.Options;
using AutoMapper;
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
        private readonly IOptionsMonitor<CreateOptions> options;
        private readonly IMapper mapper;
        private readonly ILogger<UsersController> logger;

        public UsersController(IUserService userService,
                               IOptionsMonitor<CreateOptions> options,
                               IMapper mapper,
                               ILogger<UsersController> logger)
        {
            this.userService = userService;
            this.options = options;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet("api/users/self")]
        public async Task<IActionResult> ReadSelf()
        {
            var user = await userService.ReadUserModelAsync(User);
            if(user == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<UserDto>(user)); 
        }

        [HttpPost("api/users")]
        public async Task<IActionResult> CreateUser([FromBody] UserDto dto, string onboardingId)
        {
            string password = System.Guid.NewGuid().ToString() + "!1A";
            if (Request.Headers.ContainsKey("Create"))
            {
                var value = Request.Headers["Create"];
                if (string.Equals(value, options.CurrentValue.Key))
                {
                    var applicationUser = new ApplicationUser()
                    {
                        Email = dto.Email,
                        UserName = dto.UserName
                    };

                    var result = await userService.CreateAsync(applicationUser, password);

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
                    var user = (await userService.UserManager.FindByNameAsync(username));
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