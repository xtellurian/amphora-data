using Amphora.Api.EntityFramework;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Activities;
using Amphora.Common.Stores.EFCore;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.EFCore
{
    public class ActivitiesEFStore : EFStoreBase<ActivityModel, AmphoraContext>, IEntityStore<ActivityModel>
    {
        public ActivitiesEFStore(AmphoraContext context, ILogger<ActivitiesEFStore> logger) : base(context, logger, db => db.Activities)
        {
        }
    }
}