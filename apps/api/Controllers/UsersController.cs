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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
        /// <summary>
        /// Get's logged in users information.
        /// </summary>
        [Produces(typeof(UserDto))]
        [HttpGet("api/users/self")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ReadSelf()
        {
            var user = await userService.ReadUserModelAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<UserDto>(user));
        }
        /// <summary>
        /// Creates a new User. Returns the password.
        /// </summary>
        /// <param name="dto">User parameters</param>
        [Produces(typeof(string))]
        [HttpPost("api/users")]
        public async Task<IActionResult> CreateUser([FromBody] UserDto dto)
        {
            string password = System.Guid.NewGuid().ToString() + "!1A";

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

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="userName">UserName of user to delete.</param>
        [HttpDelete("api/users/{username}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteUser_Key(string userName)
        {
            if (userName == null) return BadRequest("id/ username is required");
            var user = (await userService.UserManager.FindByNameAsync(userName));
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
    }
}