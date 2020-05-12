using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Activities;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Permissions;
using Microsoft.Extensions.Logging;

namespace Amphora.Common.Services.Activities
{
    public class ActivityRunService
    {
        private readonly IEntityStore<ActivityModel> store;
        private readonly IEntityStore<AmphoraModel> amphoraStore;
        private readonly IUserDataService userDataService;
        private readonly IPermissionService permissionService;
        private readonly IDateTimeProvider dtProvider;
        private readonly ILogger<ActivityRunService> logger;

        public ActivityRunService(IEntityStore<ActivityModel> store,
                                  IEntityStore<AmphoraModel> amphoraStore,
                                  IUserDataService userDataService,
                                  IPermissionService permissionService,
                                  IDateTimeProvider dtProvider,
                                  ILogger<ActivityRunService> logger)
        {
            this.store = store;
            this.amphoraStore = amphoraStore;
            this.userDataService = userDataService;
            this.permissionService = permissionService;
            this.dtProvider = dtProvider;
            this.logger = logger;
        }

        public async Task<EntityOperationResult<ActivityRun>> StartRunAsync(ClaimsPrincipal principal, ActivityModel activity)
        {
            var userDataReadRes = await userDataService.ReadAsync(principal);
            if (userDataReadRes.Failed || userDataReadRes.Entity == null)
            {
                return new EntityOperationResult<ActivityRun>("Unknown user");
            }

            var user = userDataReadRes.Entity;
            var authorized = await permissionService.IsAuthorizedAsync(user, user.Organisation, AccessLevels.CreateEntities);

            if (authorized)
            {
                // add a new run.
                var run = activity.NewRun(dtProvider.UtcNow, user);
                activity = await store.UpdateAsync(activity);
                return new EntityOperationResult<ActivityRun>(user, run);
            }
            else
            {
                return new EntityOperationResult<ActivityRun>(user, $"User needs write contents on Org({activity.OrganisationId})");
            }
        }

        public async Task<EntityOperationResult<ActivityRun>> ReferenceAmphoraAsync(ClaimsPrincipal principal,
                                                                                    ActivityModel activity,
                                                                                    ActivityRun run,
                                                                                    ActivityAmphoraReference amphoraRef)
        {
            var userDataReadRes = await userDataService.ReadAsync(principal);
            if (userDataReadRes.Failed || userDataReadRes.Entity == null)
            {
                return new EntityOperationResult<ActivityRun>("Unknown user");
            }

            var user = userDataReadRes.Entity;
            if (amphoraRef.AmphoraId == null)
            {
                return new EntityOperationResult<ActivityRun>(user, "Amphora Id cannot be empty in reference");
            }

            if (activity.Runs == null || !activity.Runs.Any(_ => _.Id == run.Id))
            {
                // run doesn't exist.
                return new EntityOperationResult<ActivityRun>(user, $"Run({run.Id}) does not exist in Activity({activity.Id})");
            }

            var authorized = await permissionService.IsAuthorizedAsync(user, user.Organisation, AccessLevels.CreateEntities);
            run.AmphoraReferences ??= new List<ActivityAmphoraReference>(); // ensure not null

            if (authorized && run.StartedByUserId == user.Id)
            {
                var amphora = await amphoraStore.ReadAsync(amphoraRef.AmphoraId);
                if (amphora == null)
                {
                    return new EntityOperationResult<ActivityRun>(user, $"Referenced Amphora({amphoraRef.AmphoraId}) does not exist");
                }
                else if (run.AmphoraReferences.Any(_ => _.AmphoraId == amphora.Id))
                {
                    return new EntityOperationResult<ActivityRun>(user, $"Run already references Amphora({amphora.Id})");
                }
                else
                {
                    // alls good, so let's add the reference and save the activity.
                    run.AmphoraReferences.Add(amphoraRef);
                    activity = await store.UpdateAsync(activity);
                    return new EntityOperationResult<ActivityRun>(user, run);
                }
            }
            else
            {
                return new EntityOperationResult<ActivityRun>(user, $"Permision denied")
                { WasForbidden = true, Code = 403 };
            }
        }

        public async Task<EntityOperationResult<ActivityRun>> FinishRunAsync(ClaimsPrincipal principal, ActivityModel activity, ActivityRun run, bool success = true)
        {
            var userDataReadRes = await userDataService.ReadAsync(principal);
            if (userDataReadRes.Failed || userDataReadRes.Entity == null)
            {
                return new EntityOperationResult<ActivityRun>("Unknown user");
            }

            var user = userDataReadRes.Entity;
            if (!activity.Runs.Any(_ => _.Id == run.Id))
            {
                // run doesn't exist.
                return new EntityOperationResult<ActivityRun>(user, $"Run({run.Id}) does not exist in Activity({activity.Id})");
            }

            var authorized = await permissionService.IsAuthorizedAsync(user, user.Organisation, AccessLevels.CreateEntities);

            if (authorized && user.Id == run.StartedByUserId)
            {
                run.EndTime = dtProvider.UtcNow;
                run.Success = success;
                await store.UpdateAsync(activity);
                run = activity.Runs.FirstOrDefault(_ => _.Id == run.Id);
                return new EntityOperationResult<ActivityRun>(user, run);
            }
            else
            {
                return new EntityOperationResult<ActivityRun>(user, $"User needs permissions on Run({run.Id})");
            }
        }
    }
}