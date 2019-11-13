using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.DbContexts;
using Amphora.Common.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.EFCore
{
    public class ApplicationUserStore : EFStoreBase, IEntityStore<ApplicationUser>
    {
        public ApplicationUserStore(AmphoraContext context, ILogger<ApplicationUserStore> logger) : base(context, logger)
        {
        }

        public async Task<int> CountAsync(Expression<Func<ApplicationUser, bool>> where = null)
        {
            if (where == null) return await context.Users.CountAsync();
            else return await context.Users.Where(where).CountAsync();
        }

        public Task<ApplicationUser> CreateAsync(ApplicationUser entity)
        {
            throw new NotImplementedException("Do not create a user this way. Use UserManager");
        }

        public Task DeleteAsync(ApplicationUser entity)
        {
            throw new NotImplementedException("Do not delete a user this way. Use UserManager");
        }

        public IQueryable<ApplicationUser> Query(Expression<Func<ApplicationUser, bool>> where)
        {
            return context.Users.Where(where);
        }

        public async Task<IEnumerable<ApplicationUser>> QueryAsync(Expression<Func<ApplicationUser, bool>> where)
        {
            return await this.Query(where).ToListAsync();
        }

        public Task<ApplicationUser> ReadAsync(string id)
        {
            throw new NotImplementedException("Do not read a user this way. Use UserManager");
        }

        public async Task<IList<ApplicationUser>> TopAsync()
        {
            return await context.Users.Take(6).ToListAsync();
        }

        public async Task<IList<ApplicationUser>> TopAsync(string orgId)
        {
            return await context.Users.Where(u => u.OrganisationId == orgId).Take(6).ToListAsync();
        }

        public Task<ApplicationUser> UpdateAsync(ApplicationUser entity)
        {
            throw new NotImplementedException("Do not update a user this way. Use UserManager");
        }
    }
}