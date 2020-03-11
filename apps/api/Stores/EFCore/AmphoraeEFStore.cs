using Amphora.Api.EntityFramework;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Stores.EFCore;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.EFCore
{
    public class AmphoraeEFStore : EFStoreBase<AmphoraModel, AmphoraContext>, IEntityStore<AmphoraModel>
    {
        public AmphoraeEFStore(AmphoraContext context, ILogger<AmphoraeEFStore> logger) : base(context, logger, db => db.Amphorae)
        {
        }
    }
}