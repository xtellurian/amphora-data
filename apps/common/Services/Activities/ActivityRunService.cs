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
    public class ActivityRunService : IActivityRunService
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

        public async Task<EntityOperationResult<ActivityRunModel>> StartRunAsync(ClaimsPrincipal principal, ActivityModel activity)
        {
            var userDataReadRes = await userDataService.ReadAsync(principal);
            if (userDataReadRes.Failed || userDataReadRes.Entity == null)
            {
                return new EntityOperationResult<ActivityRunModel>("Unknown user");
            }

            var user = userDataReadRes.Entity;
            var authorized = await permissionService.IsAuthorizedAsync(user, user.Organisation, AccessLevels.CreateEntities);

            if (authorized)
            {
                // add a new run.
                var run = new ActivityRunModel(activity, user, dtProvider.UtcNow);
                activity.Runs ??= new List<ActivityRunModel>(); // ensure not null
                activity.Runs.Add(run);
                activity = await store.UpdateAsync(activity);
                return new EntityOperationResult<ActivityRunModel>(user, run);
            }
            else
            {
                return new EntityOperationResult<ActivityRunModel>(user, $"User needs write contents on Org({activity.OrganisationId})");
            }
        }

        public async Task<EntityOperationResult<ActivityRunModel>> ReferenceAmphoraAsync(ClaimsPrincipal principal,
                                                                                    ActivityModel activity,
                                                                                    ActivityRunModel run,
                                                                                    ActivityAmphoraReference amphoraRef)
        {
            var userDataReadRes = await userDataService.ReadAsync(principal);
            if (userDataReadRes.Failed || userDataReadRes.Entity == null)
            {
                return new EntityOperationResult<ActivityRunModel>("Unknown user");
            }

            var user = userDataReadRes.Entity;
            if (amphoraRef.AmphoraId == null)
            {
                return new EntityOperationResult<ActivityRunModel>(user, "Amphora Id cannot be empty in reference");
            }

            if (activity.Runs == null || !activity.Runs.Any(_ => _.Id == run.Id))
            {
                // run doesn't exist.
                return new EntityOperationResult<ActivityRunModel>(user, $"Run({run.Id}) does not exist in Activity({activity.Id})");
            }

            var authorized = await permissionService.IsAuthorizedAsync(user, user.Organisation, AccessLevels.CreateEntities);
            run.AmphoraReferences ??= new List<ActivityAmphoraReference>(); // ensure not null

            if (authorized && run.StartedByUserId == user.Id)
            {
                var amphora = await amphoraStore.ReadAsync(amphoraRef.AmphoraId);
                if (amphora == null)
                {
                    return new EntityOperationResult<ActivityRunModel>(user, $"Referenced Amphora({amphoraRef.AmphoraId}) does not exist");
                }
                else if (run.AmphoraReferences.Any(_ => _.AmphoraId == amphora.Id))
                {
                    return new EntityOperationResult<ActivityRunModel>(user, $"Run already references Amphora({amphora.Id})");
                }
                else
                {
                    // alls good, so let's add the reference and save the activity.
                    run.AmphoraReferences.Add(amphoraRef);
                    activity = await store.UpdateAsync(activity);
                    return new EntityOperationResult<ActivityRunModel>(user, run);
                }
            }
            else
            {
                return new EntityOperationResult<ActivityRunModel>(user, $"Permision denied")
                { WasForbidden = true, Code = 403 };
            }
        }

        public async Task<EntityOperationResult<ActivityRunModel>> FinishRunAsync(ClaimsPrincipal principal, ActivityModel activity, ActivityRunModel run, bool success = true)
        {
            var userDataReadRes = await userDataService.ReadAsync(principal);
            if (userDataReadRes.Failed || userDataReadRes.Entity == null)
            {
                return new EntityOperationResult<ActivityRunModel>("Unknown user");
            }

            var user = userDataReadRes.Entity;
            if (!activity.Runs.Any(_ => _.Id == run.Id))
            {
                // run doesn't exist.
                return new EntityOperationResult<ActivityRunModel>(user, $"Run({run.Id}) does not exist in Activity({activity.Id})");
            }

            var authorized = await permissionService.IsAuthorizedAsync(user, user.Organisation, AccessLevels.CreateEntities);

            if (authorized && user.Id == run.StartedByUserId)
            {
                run.EndTime = dtProvider.UtcNow;
                run.Success = success;
                await store.UpdateAsync(activity);
                run = activity.Runs.FirstOrDefault(_ => _.Id == run.Id);
                return new EntityOperationResult<ActivityRunModel>(user, run);
            }
            else
            {
                return new EntityOperationResult<ActivityRunModel>(user, $"User needs permissions on Run({run.Id})");
            }
        }
    }
}