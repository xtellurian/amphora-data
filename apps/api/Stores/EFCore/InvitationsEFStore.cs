using Amphora.Api.Contracts;
using Amphora.Api.EntityFramework;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Platform;
using Amphora.Common.Stores.EFCore;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.EFCore
{
    public class InvitationsEFStore : EFStoreBase<InvitationModel, AmphoraContext>, IEntityStore<InvitationModel>
    {
        public InvitationsEFStore(AmphoraContext context, ILogger<InvitationsEFStore> logger) : base(context, logger, db => db.Invitations)
        {
        }
    }
}