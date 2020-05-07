using System.Threading.Tasks;
using Amphora.Common.Models.Dtos.Users;
using Amphora.Identity.Contracts;
using Amphora.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Amphora.Identity.Controllers
{
    [Route("api/users")]
    public class UsersController : Controller
    {
        private readonly IUserService userService;
        private readonly ILogger<UsersController> logger;
        private readonly IWebHostEnvironment environment;
        private readonly UserManager<ApplicationUser> userManager;

        public UsersController(IUserService userService,
                               ILogger<UsersController> logger,
                               IWebHostEnvironment environment,
                               UserManager<ApplicationUser> userManager)
        {
            this.userService = userService;
            this.logger = logger;
            this.environment = environment;
            this.userManager = userManager;
        }

        /// <summary>
        /// Creates a new User. Returns the password.
        /// </summary>
        /// <param name="user">User parameters.</param>
        /// <returns> A.</returns>
        [Produces(typeof(AmphoraUser))]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAmphoraUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var applicationUser = new ApplicationUser()
            {
                Email = user.Email,
                UserName = user.UserName,
                About = user.About,
                FullName = user.FullName
            };

            if (IsTestUser(applicationUser))
            {
                logger.LogWarning($"Automatically confirming email for {applicationUser.Email}");
                applicationUser.EmailConfirmed = true;
            }

            // var invitation = await invitationService.GetInvitationByEmailAsync(user.Email.ToUpper());

            var result = await userService.CreateAsync(applicationUser, null!, user.Password!); // invitation goes here

            if (result.Succeeded && result.Entity != null)
            {
                var dto = new AmphoraUser()
                {
                    About = result.Entity.About,
                    Email = result.Entity.Email,
                    UserName = result.Entity.UserName,
                    Id = result.Entity.Id,
                    OrganisationId = result.Entity.OrganisationId,
                    FullName = result.Entity.FullName,
                };

                logger.LogInformation($"Created user: {JsonConvert.SerializeObject(dto, Formatting.Indented)}");

                return Ok(dto);
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
        /// <param name="userName">Username to delete.</param>
        /// <returns> Ok (200) if successfully deleted.</returns>
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete(string userName)
        {
            var user = await userManager.FindByNameAsync(userName);
            var res = await userService.DeleteAsync(User, user);
            if (res.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(res.Message);
            }
        }

        private bool IsTestUser(ApplicationUser user)
        {
            return environment.IsDevelopment() &&
            (
                (user.Email?.ToLower().EndsWith("@amphoradata.com") ?? false)
                || (user.Email?.ToLower().EndsWith("@example.org") ?? false));
        }
    }
}