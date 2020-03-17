using Amphora.Common.Contracts;
using Amphora.Common.Models.Users;
using Amphora.Common.Stores.EFCore;
using Amphora.Identity.EntityFramework;
using Amphora.Identity.Models;
using Microsoft.Extensions.Logging;

namespace Amphora.Identity.Stores
{
    public class UsersEFStore : EFStoreBase<ApplicationUser, IdentityContext>, IEntityStore<ApplicationUser>
    {
        public UsersEFStore(IdentityContext context,
                            ILogger<EFStoreBase<ApplicationUser, IdentityContext>> logger)
                            : base(context, logger, c => c.Users)
        {
        }
    }
}