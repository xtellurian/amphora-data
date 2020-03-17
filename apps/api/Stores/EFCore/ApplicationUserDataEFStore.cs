using Amphora.Api.EntityFramework;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Users;
using Amphora.Common.Stores.EFCore;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.EFCore
{
    public class ApplicationUserDataEFStore : EFStoreBase<ApplicationUserDataModel, AmphoraContext>, IEntityStore<ApplicationUserDataModel>
    {
        public ApplicationUserDataEFStore(AmphoraContext context, ILogger<ApplicationUserDataEFStore> logger) : base(context, logger, db => db.UserData)
        {
        }
    }
}