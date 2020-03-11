using Amphora.Api.EntityFramework;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Purchases;
using Amphora.Common.Stores.EFCore;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.EFCore
{
    public class CommissionsEFStore : EFStoreBase<CommissionModel, AmphoraContext>, IEntityStore<CommissionModel>
    {
        public CommissionsEFStore(AmphoraContext context, ILogger<CommissionsEFStore> logger) : base(context, logger, db => db.Commissions)
        {
        }
    }
}