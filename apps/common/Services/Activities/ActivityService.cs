using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Activities;
using Amphora.Common.Models.Permissions;
using Microsoft.Extensions.Logging;

namespace Amphora.Common.Services.Activities
{
    public class ActivityService
    {
        private readonly IEntityStore<ActivityModel> store;
        private readonly IUserDataService userDataService;
        private readonly IPermissionService permissionService;
        private readonly ILogger<ActivityService> logger;

        public ActivityService(IEntityStore<ActivityModel> store,
                               IUserDataService userDataService,
                               IPermissionService permissionService,
                               ILogger<ActivityService> logger)
        {
            this.store = store;
            this.userDataService = userDataService;
            this.permissionService = permissionService;
            this.logger = logger;
        }

        public async Task<EntityOperationResult<ActivityModel>> CreateAsync(ClaimsPrincipal principal, ActivityModel activity)
        {
            var userDataReadRes = await userDataService.ReadAsync(principal);
            if (userDataReadRes.Failed || userDataReadRes.Entity == null)
            {
                return new EntityOperationResult<ActivityModel>("Unknown user");
            }

            var user = userDataReadRes.Entity;
            // now make sure to set the organisation to this users org.
            activity.OrganisationId = user.OrganisationId;
            activity.Organisation = user.Organisation;

            var authorized = await permissionService.IsAuthorizedAsync(user, user.Organisation, AccessLevels.CreateEntities);
            if (!authorized)
            {
                return new EntityOperationResult<ActivityModel>(user, $"Permission denied. Write Contents is required on Org({activity.OrganisationId}).")
                { WasForbidden = true, Code = 403 };
            }

            logger.LogInformation($"User({user.UserName}) is creating an activity for Org({activity.OrganisationId}).");
            // create the activity in the database.
            activity = await store.CreateAsync(activity);

            if (activity != null)
            {
                return new EntityOperationResult<ActivityModel>(user, activity);
            }
            else
            {
                return new EntityOperationResult<ActivityModel>(user, "Failed to save activity in the database.");
            }
        }

        public async Task<EntityOperationResult<ActivityModel>> ReadAsync(ClaimsPrincipal principal, string idOrName)
        {
            var userDataReadRes = await userDataService.ReadAsync(principal);
            if (userDataReadRes.Failed || userDataReadRes.Entity == null)
            {
                return new EntityOperationResult<ActivityModel>("Unknown user");
            }

            var user = userDataReadRes.Entity;

            logger.LogInformation($"User({user.UserName}) is reading Activity({idOrName}).");
            // create the activity in the database.
            var activity = await store.ReadAsync(idOrName);
            // try by name, if activity is null
            activity ??= (await store.QueryAsync(_ => _.Name == idOrName))?.FirstOrDefault();
            // if still null, return not found
            if (activity == null)
            {
                return new EntityOperationResult<ActivityModel>(user, $"Activity({idOrName}) not found");
            }
            else
            {
                var authorized = await permissionService.IsAuthorizedAsync(user, user.Organisation, AccessLevels.CreateEntities);
                if (authorized)
                {
                    return new EntityOperationResult<ActivityModel>(user, activity);
                }
                else
                {
                    return new EntityOperationResult<ActivityModel>(
                        user, $"Permission denied. Create Entities is required on Org({user.OrganisationId}).")
                    { WasForbidden = true, Code = 403 };
                }
            }
        }

        public async Task<EntityOperationResult<ActivityModel>> DeleteAsync(ClaimsPrincipal principal, ActivityModel activity)
        {
            var userDataReadRes = await userDataService.ReadAsync(principal);
            if (userDataReadRes.Failed || userDataReadRes.Entity == null)
            {
                return new EntityOperationResult<ActivityModel>("Unknown user");
            }

            var user = userDataReadRes.Entity;
            if (activity == null)
            {
                return new EntityOperationResult<ActivityModel>(user, "Activity(null) not found");
            }

            logger.LogInformation($"User({user.UserName}) is deleting Activity({activity.Id}).");

            var authorized = await permissionService.IsAuthorizedAsync(user, user.Organisation, AccessLevels.Update);
            if (authorized)
            {
                await store.DeleteAsync(activity);
                return new EntityOperationResult<ActivityModel>(user, true);
            }
            else
            {
                return new EntityOperationResult<ActivityModel>(
                    user, $"Permission denied. Update is required on Org({user.OrganisationId}).")
                { WasForbidden = true, Code = 403 };
            }
        }
    }
}