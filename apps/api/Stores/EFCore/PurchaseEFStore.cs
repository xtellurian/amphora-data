using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.DbContexts;
using Amphora.Common.Models.Purchases;
using Microsoft.EntityFrameworkCore;

namespace Amphora.Api.Stores.EFCore
{
    public class PurchaseEFStore : EFStoreBase, IEntityStore<PurchaseModel>
    {
        private readonly AmphoraContext context;

        public PurchaseEFStore(AmphoraContext context)
        {
            this.context = context;
        }
        public async Task<PurchaseModel> CreateAsync(PurchaseModel entity)
        {
            var x = await this.context.Purchases.AddAsync(entity);
            await this.context.SaveChangesAsync();
            return x.Entity;
        }

        public async Task DeleteAsync(PurchaseModel entity)
        {
            var e = await this.context.Purchases.SingleOrDefaultAsync(a => a.Id == entity.Id);
            this.context.Purchases.Remove(e);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PurchaseModel>> QueryAsync(Expression<Func<PurchaseModel, bool>> where)
        {
            return await context.Purchases.Where(where).ToListAsync();
        }

        public async Task<PurchaseModel> ReadAsync(string id, bool includeChildren = false)
        {
            var result = await context.Purchases.SingleOrDefaultAsync(a => a.AmphoraId == id);
            return result;
        }

        public async Task<IList<PurchaseModel>> TopAsync()
        {
            var x = await context.Purchases.Take(5).ToListAsync();
            return new List<PurchaseModel>(x);
        }

        public async Task<IList<PurchaseModel>> TopAsync(string orgId)
        {
            var x = await context.Purchases.Where(t => t.PurchasedByOrganisationId == orgId).Take(5).ToListAsync();
            return new List<PurchaseModel>(x);
        }

        public async Task<PurchaseModel> UpdateAsync(PurchaseModel entity)
        {
            base.OnUpdateEntity(entity);
            var o = context.Purchases.Update(entity);
            await context.SaveChangesAsync();
            return o.Entity;
        }

        public async Task<int> CountAsync()
        {
            return await this.context.Purchases.CountAsync();
        }
    }
}