using Amphora.Api.Contracts;
using Amphora.Api.EntityFramework;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Purchases;
using Amphora.Common.Stores.EFCore;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.EFCore
{
    public class PurchaseEFStore : EFStoreBase<PurchaseModel, AmphoraContext>, IEntityStore<PurchaseModel>
    {
        public PurchaseEFStore(AmphoraContext context, ILogger<PurchaseEFStore> logger) : base(context, logger, db => db.Purchases)
        {
        }
    }
}