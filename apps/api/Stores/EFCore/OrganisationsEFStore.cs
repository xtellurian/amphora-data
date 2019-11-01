using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.DbContexts;
using Amphora.Common.Models.Organisations;
using Microsoft.EntityFrameworkCore;

namespace Amphora.Api.Stores.EFCore
{
    public class OrganisationsEFStore : EFStoreBase, IEntityStore<OrganisationModel>
    {
        private readonly AmphoraContext context;

        public OrganisationsEFStore(AmphoraContext context)
        {
            this.context = context;
        }
        public async Task<OrganisationModel> CreateAsync(OrganisationModel entity)
        {
            var r = await this.context.Organisations.AddAsync(entity);
            await context.SaveChangesAsync();
            return r.Entity;
        }

        public async Task DeleteAsync(OrganisationModel entity)
        {
            context.Organisations.Remove(entity);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrganisationModel>> QueryAsync(Expression<Func<OrganisationModel, bool>> where)
        {
            return await context.Organisations.Where(where).ToListAsync();
        }

        public async Task<OrganisationModel> ReadAsync(string id)
        {
            return await context.Organisations.SingleOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IList<OrganisationModel>> TopAsync()
        {
            return await context.Organisations.Take(10).ToListAsync();
        }

        public async Task<IList<OrganisationModel>> TopAsync(string orgId)
        {
            var r = context.Organisations.Where(o => o.Id == orgId).Take(10);
            return new List<OrganisationModel>(await r.ToListAsync());
        }

        public async Task<OrganisationModel> UpdateAsync(OrganisationModel entity)
        {
            base.OnUpdateEntity(entity);
            var e = context.Organisations.Update(entity);
            await context.SaveChangesAsync();
            return e.Entity;
        }

        public async Task<int> CountAsync()
        {
            return await this.context.Organisations.CountAsync();
        }
    }
}