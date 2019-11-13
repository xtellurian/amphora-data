using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.DbContexts;
using Amphora.Common.Models.Platform;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.EFCore
{
    public class InvitationsEFStore : EFStoreBase, IEntityStore<InvitationModel>
    {
        public InvitationsEFStore(AmphoraContext context, ILogger<InvitationsEFStore> logger): base(context, logger)
        {
        }
        public async Task<int> CountAsync(Expression<Func<InvitationModel, bool>> where = null)
        {
            if(where == null) return await context.Invitations.CountAsync();
            else return await context.Invitations.Where(where).CountAsync();
        }

        public async Task<InvitationModel> CreateAsync(InvitationModel entity)
        {
            var x = await this.context.Invitations.AddAsync(entity);
            await this.context.SaveChangesAsync();
            return x.Entity;
        }

        public async Task DeleteAsync(InvitationModel entity)
        {
            var e = await this.context.Invitations.SingleOrDefaultAsync(a => a.Id == entity.Id);
            this.context.Invitations.Remove(e);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<InvitationModel>> QueryAsync(Expression<Func<InvitationModel, bool>> where)
        {
            return await context.Invitations.Where(where).ToListAsync();
        }

        public IQueryable<InvitationModel> Query(Expression<Func<InvitationModel, bool>> where)
        {
            return context.Invitations.Where(where);
        }
        public async Task<InvitationModel> ReadAsync(string id)
        {
            var result = await context.Invitations.SingleOrDefaultAsync(a => a.Id == id);
            return result;
        }

        public async Task<IList<InvitationModel>> TopAsync()
        {
            var x = await context.Invitations.Take(10).ToListAsync();
            return new List<InvitationModel>(x);
        }

        public async Task<IList<InvitationModel>> TopAsync(string orgId)
        {
            var x = await context.Invitations.Where(i => i.TargetOrganisationId == orgId).Take(10).ToListAsync();
            return new List<InvitationModel>(x);
        }

        public async Task<InvitationModel> UpdateAsync(InvitationModel entity)
        {
            base.OnUpdateEntity(entity);
            await context.SaveChangesAsync();
            var existing = await context.Invitations.SingleOrDefaultAsync(a => a.Id == entity.Id);
            return existing;
        }
    }
}