using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Models;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> logger;
        private readonly IEntityStore<Organisation> orgStore;
        private readonly IUserManager<ApplicationUser> userManager;

        public UserService(ILogger<UserService> logger,
                           IEntityStore<Organisation> orgStore,
                           IUserManager<ApplicationUser> userManager)
        {
            this.logger = logger;
            this.orgStore = orgStore;
            this.userManager = userManager;
        }

        public async Task<EntityOperationResult<ApplicationUser>> CreateUserAsync(ApplicationUser user, string password)
        {
            if(user.Validate())
            {
                var org = orgStore.ReadAsync(user.OrganisationId);
                if(org == null)
                {
                    return new EntityOperationResult<ApplicationUser>("Unknown Organisation");
                }
                var result = await userManager.CreateAsync(user, password);
                if(result.Succeeded)
                {
                    return new EntityOperationResult<ApplicationUser>(user);
                }
                else
                {
                    return new EntityOperationResult<ApplicationUser>(result.Errors.Select(e => e.Description));
                }
            }
            else
            {
                return new EntityOperationResult<ApplicationUser>("Invalid User");
            }

        }
    }
}