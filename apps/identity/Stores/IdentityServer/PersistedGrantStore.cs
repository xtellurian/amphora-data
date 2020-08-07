using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Identity.EntityFramework;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;

namespace Amphora.Identity.Stores.IdentityServer
{
    public class PersistedGrantStore : IPersistedGrantStore
    {
        private readonly IdentityContext context;

        public PersistedGrantStore(IdentityContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            return await context.Grants.Where(_ => _.SubjectId == subjectId).ToListAsync();
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            return await context.Grants.FindAsync(key);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            var grants = await context.Grants.Where(_ => _.SubjectId == subjectId && _.ClientId == clientId).ToListAsync();
            foreach (var g in grants)
            {
                context.Remove(g);
            }

            await context.SaveChangesAsync();
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            var grants = await context.Grants.Where(_ =>
                    _.SubjectId == subjectId &&
                    _.ClientId == clientId &&
                    _.Type == type)
                .ToListAsync();
            foreach (var g in grants)
            {
                context.Remove(g);
            }

            await context.SaveChangesAsync();
        }

        public async Task RemoveAsync(string key)
        {
            var grant = await context.Grants.FindAsync(key);
            context.Grants.Remove(grant);
            await context.SaveChangesAsync();
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            context.Grants.Add(grant);
            await context.SaveChangesAsync();
        }
    }
}