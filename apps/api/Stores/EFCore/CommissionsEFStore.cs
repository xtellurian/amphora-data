using Amphora.Api.Contracts;
using Amphora.Api.EntityFramework;
using Amphora.Common.Models.Purchases;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.EFCore
{
    public class CommissionsEFStore : EFStoreBase<CommissionModel>, IEntityStore<CommissionModel>
    {
        public CommissionsEFStore(AmphoraContext context, ILogger<CommissionsEFStore> logger) : base(context, logger, db => db.Commissions)
        {
        }
    }
}