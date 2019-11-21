using System;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.DbContexts;
using Amphora.Common.Models.Users;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.EFCore
{
    public class ApplicationUserStore : EFStoreBase<ApplicationUser>, IEntityStore<ApplicationUser>
    {
        public ApplicationUserStore(AmphoraContext context, ILogger<ApplicationUserStore> logger) : base(context, logger, db => db.Users)
        {
        }

        public override Task<ApplicationUser> CreateAsync(ApplicationUser entity)
        {
            throw new NotImplementedException("Do not create a user this way. Use UserManager");
        }

        public override Task DeleteAsync(ApplicationUser entity)
        {
            throw new NotImplementedException("Do not delete a user this way. Use UserManager");
        }

        public override Task<ApplicationUser> ReadAsync(string id)
        {
            throw new NotImplementedException("Do not read a user this way. Use UserManager");
        }
    }
}