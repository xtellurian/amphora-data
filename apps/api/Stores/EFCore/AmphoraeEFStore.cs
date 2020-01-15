using Amphora.Api.Contracts;
using Amphora.Api.EntityFramework;
using Amphora.Common.Models.Amphorae;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.EFCore
{
    public class AmphoraeEFStore : EFStoreBase<AmphoraModel>, IEntityStore<AmphoraModel>
    {
        public AmphoraeEFStore(AmphoraContext context, ILogger<AmphoraeEFStore> logger) : base(context, logger, db => db.Amphorae)
        {
        }
    }
}