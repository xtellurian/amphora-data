using Amphora.Api.Contracts;
using Amphora.Api.EntityFramework;
using Amphora.Common.Models.Permissions;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.EFCore
{
    public class RestrictionsEFStore : EFStoreBase<RestrictionModel>, IEntityStore<RestrictionModel>
    {
        public RestrictionsEFStore(AmphoraContext context, ILogger<EFStoreBase<RestrictionModel>> logger) : base(context, logger, _ => _.Restrictions)
        {
        }
    }
}