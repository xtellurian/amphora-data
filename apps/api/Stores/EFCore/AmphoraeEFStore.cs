using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.DbContexts;
using Amphora.Common.Models.Amphorae;
using Microsoft.EntityFrameworkCore;

namespace Amphora.Api.Stores.EFCore
{
    public class AmphoraeEFStore : EFStoreBase, IEntityStore<AmphoraModel>
    {
        private readonly AmphoraContext context;

        public AmphoraeEFStore(AmphoraContext context)
        {
            this.context = context;
        }
        public async Task<AmphoraModel> CreateAsync(AmphoraModel entity)
        {
            var x = await this.context.Amphorae.AddAsync(entity);
            await this.context.SaveChangesAsync();
            return x.Entity;
        }

        public async Task DeleteAsync(AmphoraModel entity)
        {
            var e = await this.context.Amphorae.SingleOrDefaultAsync(a => a.Id == entity.Id);
            this.context.Amphorae.Remove(e);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AmphoraModel>> QueryAsync(Expression<Func<AmphoraModel, bool>> where)
        {
            return await context.Amphorae.Where(where).ToListAsync();
        }

        public async Task<AmphoraModel> ReadAsync(string id)
        {
            var result = await context.Amphorae.SingleOrDefaultAsync(a => a.Id == id);
            return result;
        }

        public async Task<AmphoraModel> ReadAsync(string id, string orgId)
        {
            var result = await context.Amphorae.SingleOrDefaultAsync(a => a.Id == id && a.OrganisationId == orgId);
            return result;
        }

        public async Task<IList<AmphoraModel>> TopAsync()
        {
            var x = await context.Amphorae.ToListAsync();
            return new List<AmphoraModel>(x);
        }

        public async Task<IList<AmphoraModel>> TopAsync(string orgId)
        {
            var x = context.Amphorae.Where(a => a.OrganisationId == orgId);
            return new List<AmphoraModel>(await x.ToListAsync());
        }

        public async Task<AmphoraModel> UpdateAsync(AmphoraModel entity)
        {
            base.OnUpdateEntity(entity);
            await context.SaveChangesAsync();
            var existing = await context.Amphorae.SingleOrDefaultAsync(a => a.Id == entity.Id);
            return existing;
        }

        public async Task<int> CountAsync()
        {
            return await this.context.Amphorae.CountAsync();
        }

    }
}