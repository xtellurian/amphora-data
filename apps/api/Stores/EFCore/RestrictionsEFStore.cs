using Amphora.Api.EntityFramework;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Permissions;
using Amphora.Common.Stores.EFCore;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.EFCore
{
    public class RestrictionsEFStore : EFStoreBase<RestrictionModel, AmphoraContext>, IEntityStore<RestrictionModel>
    {
        public RestrictionsEFStore(AmphoraContext context,
                                   ILogger<RestrictionsEFStore> logger)
                                   : base(context, logger, _ => _.Restrictions)
        {
        }
    }
}