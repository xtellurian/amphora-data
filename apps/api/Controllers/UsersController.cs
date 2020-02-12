using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Common.Models.Dtos.Users;
using Amphora.Common.Models.Users;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NSwag.Annotations;

namespace Amphora.Api.Controllers
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    [OpenApiTag("Users")]
    [Route("api/users")]
    public class UsersController : Controller
    {
        private readonly IUserService userService;
        private readonly IInvitationService invitationService;
        private readonly IOptionsMonitor<CreateOptions> options;
        private readonly IWebHostEnvironment environment;
        private readonly IMapper mapper;
        private readonly ILogger<UsersController> logger;

        public UsersController(IUserService userService,
                               IInvitationService invitationService,
                               IOptionsMonitor<CreateOptions> options,
                               IWebHostEnvironment environment,
                               IMapper mapper,
                               ILogger<UsersController> logger)
        {
            this.userService = userService;
            this.invitationService = invitationService;
            this.options = options;
            this.environment = environment;
            this.mapper = mapper;
            this.logger = logger;
        }

        /// <summary>
        /// Get's logged in users information.
        /// </summary>
        /// <returns> Your own details. </returns>
        [Produces(typeof(AmphoraUser))]
        [HttpGet("self")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ReadSelf()
        {
            var user = await userService.ReadUserModelAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<AmphoraUser>(user));
        }

        /// <summary>
        /// Creates a new User. Returns the password.
        /// </summary>
        /// <param name="user">User parameters.</param>
        /// <returns> A password string.</returns>
        [Produces(typeof(string))]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AmphoraUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            string password = System.Guid.NewGuid().ToString() + "!1A";

            var applicationUser = new ApplicationUser()
            {
                Email = user.Email,
                UserName = user.UserName
            };

            var invitation = await invitationService.GetInvitationByEmailAsync(user.Email.ToUpper());

            var result = await userService.CreateAsync(applicationUser, invitation, password);

            if (result.Succeeded)
            {
                if (IsTestUser(result.Entity))
                {
                    logger.LogWarning($"Automatically confirming email for {result.Entity.Email}");
                    result.Entity.EmailConfirmed = true;
                    var updateRes = await userService.UserManager.UpdateAsync(result.Entity);
                }

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
        /// Deletes a user.
        /// </summary>
        /// <param name="userName">UserName of user to delete.</param>
        /// <returns> Empty.</returns>
        [HttpDelete("{username}")]
        [OpenApiIgnore]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete(string userName)
        {
            if (userName == null) { return BadRequest("username is required"); }
            var user = await userService.UserManager.FindByNameAsync(userName);
            if (user == null) { return NotFound(); }
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

        private bool IsTestUser(ApplicationUser user)
        {
            return environment.IsDevelopment() && (user.Email?.ToLower().EndsWith("@amphoradata.com") ?? false);
        }
    }
}