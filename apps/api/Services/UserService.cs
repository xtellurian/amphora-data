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
        private readonly IUserManager userManager;

        public UserService(ILogger<UserService> logger,
                           IEntityStore<Organisation> orgStore,
                           IUserManager userManager)
        {
            this.logger = logger;
            this.orgStore = orgStore;
            this.userManager = userManager;
        }

        public async Task<EntityOperationResult<IApplicationUser>> CreateAsync(IApplicationUser user, string password)
        {
            if(user.Validate())
            {
                var org = await orgStore.ReadAsync(user.OrganisationId);
                if(org == null)
                {
                    return new EntityOperationResult<IApplicationUser>("Unknown Organisation");
                }
                var result = await userManager.CreateAsync(user, password);
                if(result.Succeeded)
                {
                    return new EntityOperationResult<IApplicationUser>(user);
                }
                else
                {
                    return new EntityOperationResult<IApplicationUser>(result.Errors.Select(e => e.Description));
                }
            }
            else
            {
                return new EntityOperationResult<IApplicationUser>("Invalid User");
            }

        }

        public async Task<EntityOperationResult<IApplicationUser>> DeleteAsync(IApplicationUser user)
        {
            // todo - permissions
            var result = await userManager.DeleteAsync(user);
            if(result.Succeeded)
            {
                return new EntityOperationResult<IApplicationUser>();
            }
            else
            {
                return new EntityOperationResult<IApplicationUser>(result.Errors.Select(e => e.Description));
            }
        }
    }
}