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
using NSwag.Annotations;
using Amphora.Api.AspNet;

namespace Amphora.Api.Controllers
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    public class UsersController : Controller
    {
        private readonly IUserService userService;
        private readonly IInvitationService invitationService;
        private readonly IOptionsMonitor<CreateOptions> options;
        private readonly IMapper mapper;
        private readonly ILogger<UsersController> logger;

        public UsersController(IUserService userService,
                               IInvitationService invitationService,
                               IOptionsMonitor<CreateOptions> options,
                               IMapper mapper,
                               ILogger<UsersController> logger)
        {
            this.userService = userService;
            this.invitationService = invitationService;
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
        public async Task<IActionResult> Create([FromBody] UserDto dto)
        {
            if(! ModelState.IsValid)
            {
                return BadRequest();
            }
            string password = System.Guid.NewGuid().ToString() + "!1A";

            var applicationUser = new ApplicationUser()
            {
                Email = dto.Email,
                UserName = dto.UserName
            };

            var invitation = await invitationService.GetInvitationByEmailAsync(dto.Email.ToUpper());
            if(invitation == null)
            {
                return BadRequest($"{dto.Email} has not been invited to Amphora Data");
            }

            var result = await userService.CreateAsync(applicationUser, invitation, password);

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
        [OpenApiIgnore]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete(string userName)
        {
            if (userName == null) return BadRequest("username is required");
            var user = (await userService.UserManager.FindByNameAsync(userName));
            if (user == null) return NotFound();
            var result = await userService.DeleteAsync(User, user);
            if (result.Succeeded)
            {
                return Ok();
            }
            else if (result.WasForbidden)
            {
                return StatusCode(403, result.Message);
            }
            else
            {
                logger.LogError("Failed to create user!");
                return BadRequest(result.Message);
            }

        }
    }
}