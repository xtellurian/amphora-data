using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.DbContexts;
using Amphora.Common.Models.Organisations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.EFCore
{
    public class OrganisationsEFStore : EFStoreBase, IEntityStore<OrganisationModel>
    {

        public OrganisationsEFStore(AmphoraContext context, ILogger<OrganisationsEFStore> logger): base(context, logger)
        {
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

         public IQueryable<OrganisationModel> Query(Expression<Func<OrganisationModel, bool>> where)
        {
            return context.Organisations.Where(where);
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
        public async Task<int> CountAsync(Expression<Func<OrganisationModel, bool>> where = null)
        {
            if(where == null) return await context.Organisations.CountAsync();
            else return await context.Organisations.Where(where).CountAsync();
        }
    }
}