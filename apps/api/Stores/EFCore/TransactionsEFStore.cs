using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.DbContexts;
using Amphora.Common.Models.Transactions;
using Microsoft.EntityFrameworkCore;

namespace Amphora.Api.Stores.EFCore
{
    public class TransactionEFStore : IEntityStore<TransactionModel>
    {
        private readonly AmphoraContext context;

        public TransactionEFStore(AmphoraContext context)
        {
            this.context = context;
        }
        public async Task<TransactionModel> CreateAsync(TransactionModel entity)
        {
            var x = await this.context.Transactions.AddAsync(entity);
            await this.context.SaveChangesAsync();
            return x.Entity;
        }

        public async Task DeleteAsync(TransactionModel entity)
        {
            var e = await this.context.Transactions.SingleOrDefaultAsync(a => a.Id == entity.Id);
            this.context.Transactions.Remove(e);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TransactionModel>> QueryAsync(Expression<Func<TransactionModel, bool>> where)
        {
            return await context.Transactions.Where(where).ToListAsync();
        }

        public async Task<TransactionModel> ReadAsync(string id, bool includeChildren = false)
        {
            var result = await context.Transactions.SingleOrDefaultAsync(a => a.AmphoraId == id);
            return result;
        }

        public async Task<IList<TransactionModel>> TopAsync()
        {
            var x = await context.Transactions.Take(5).ToListAsync();
            return new List<TransactionModel>(x);
        }

        public async Task<IList<TransactionModel>> TopAsync(string orgId)
        {
            var x = await context.Transactions.Where(t => t.OrganisationId == orgId).Take(5).ToListAsync();
            return new List<TransactionModel>(x);
        }

        public async Task<TransactionModel> UpdateAsync(TransactionModel entity)
        {
            var o = context.Transactions.Update(entity);
            await context.SaveChangesAsync();
            return o.Entity;
        }
    }
}