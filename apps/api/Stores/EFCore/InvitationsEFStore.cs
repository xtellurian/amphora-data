using Amphora.Api.Contracts;
using Amphora.Api.EntityFramework;
using Amphora.Common.Models.Platform;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.EFCore
{
    public class InvitationsEFStore : EFStoreBase<InvitationModel>, IEntityStore<InvitationModel>
    {
        public InvitationsEFStore(AmphoraContext context, ILogger<InvitationsEFStore> logger) : base(context, logger, db => db.Invitations)
        {
        }
    }
}