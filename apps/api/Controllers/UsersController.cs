using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Dtos.Users;
using Amphora.Common.Models.Users;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IUserDataService userDataService;
        private readonly IIdentityService identityService;
        private readonly IInvitationService invitationService;
        private readonly IOptionsMonitor<CreateOptions> options;
        private readonly IWebHostEnvironment environment;
        private readonly IMapper mapper;
        private readonly ILogger<UsersController> logger;

        public UsersController(IUserDataService userDataService,
                               IIdentityService identityService,
                               IInvitationService invitationService,
                               IOptionsMonitor<CreateOptions> options,
                               IWebHostEnvironment environment,
                               IMapper mapper,
                               ILogger<UsersController> logger)
        {
            this.userDataService = userDataService;
            this.identityService = identityService;
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
        [CommonAuthorize]
        public async Task<IActionResult> ReadSelf()
        {
            var userReadRes = await userDataService.ReadAsync(User);
            if (userReadRes.Succeeded)
            {
                return Ok(mapper.Map<AmphoraUser>(userReadRes.Entity));
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Creates a new User. Returns the password.
        /// </summary>
        /// <param name="user">User parameters.</param>
        /// <returns> The info for the created user.</returns>
        [Produces(typeof(AmphoraUser))]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAmphoraUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await identityService.CreateUser(user, user.Password);

            if (result.Succeeded)
            {
                var invitation = await invitationService.GetInvitationByEmailAsync(user.Email.ToUpper());

                var applicationUser = new ApplicationUserDataModel(result.Entity.Id, result.Entity.UserName, result.Entity.About,
                    new ContactInformation(result.Entity.Email, result.Entity.FullName));

                if (IsTestUser(result.Entity))
                {
                    logger.LogWarning($"Automatically confirming email for {applicationUser.ContactInformation.Email}");
                    applicationUser.ContactInformation.EmailConfirmed = true;
                }

                var createRes = await userDataService.CreateAsync(User, applicationUser);
                if (createRes.Succeeded)
                {
                    return Ok(mapper.Map<AmphoraUser>(createRes.Entity));
                }
                else
                {
                    return BadRequest();
                }
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
        [CommonAuthorize]
        public async Task<IActionResult> Delete(string userName)
        {
            if (userName == null) { return BadRequest("username is required"); }
            var user = await userDataService.Query(User, _ => _.UserName == userName).FirstOrDefaultAsync();
            if (user == null) { return NotFound(); }
            var result = await identityService.DeleteUser(User, user);
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

        private bool IsTestUser(AmphoraUser user)
        {
            return environment.IsDevelopment() && (user.Email?.ToLower().EndsWith("@amphoradata.com") ?? false);
        }
    }
}