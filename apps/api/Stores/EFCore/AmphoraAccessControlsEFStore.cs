using Amphora.Api.EntityFramework;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Stores.EFCore;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.EFCore
{
    public class AmphoraAccessControlsEFStore : EFStoreBase<AmphoraAccessControlModel, AmphoraContext>, IEntityStore<AmphoraAccessControlModel>
    {
        public AmphoraAccessControlsEFStore(AmphoraContext context, ILogger<AmphoraAccessControlsEFStore> logger)
        : base(context, logger, db => db.AmphoraAccessControls)
        {
        }
    }
}