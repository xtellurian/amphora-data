using Amphora.Api.EntityFramework;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Stores.EFCore;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.EFCore
{
    public class TermsOfUseEFStore : EFStoreBase<TermsOfUseModel, AmphoraContext>, IEntityStore<TermsOfUseModel>
    {
        public TermsOfUseEFStore(AmphoraContext context, ILogger<TermsOfUseEFStore> logger) : base(context, logger, db => db.TermsOfUse)
        {
        }
    }
}