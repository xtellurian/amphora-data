using Amphora.Api.Contracts;
using Amphora.Api.EntityFramework;
using Amphora.Common.Models.Signals;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.EFCore
{
    public class SignalsEFStore : EFStoreBase<SignalModel>, IEntityStore<SignalModel>
    {
        public SignalsEFStore(AmphoraContext context, ILogger<SignalsEFStore> logger) : base(context, logger, db => db.Signals)
        {
        }
    }
}