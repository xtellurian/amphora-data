using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Identity.EntityFramework;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Amphora.Identity.Stores.IdentityServer
{
    public class PersistedGrantStore : IPersistedGrantStore
    {
        private readonly IdentityContext context;
        private readonly ILogger<PersistedGrantStore> logger;

        public PersistedGrantStore(IdentityContext context, ILogger<PersistedGrantStore> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            return await context.Grants.Where(_ => _.SubjectId == subjectId).ToListAsync();
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            return await context.Grants.FindAsync(key);
        }

        /// <summary>
        /// Tries to save changes.
        /// Logs an critical error on failure.
        /// Will rethrow any exceptions.
        /// </summary>
        private async Task SaveChangesWithLoggerAsync(IdentityContext context)
        {
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, ex.Message);
                throw ex;
            }
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            var grants = await context.Grants.Where(_ => _.SubjectId == subjectId && _.ClientId == clientId).ToListAsync();
            foreach (var g in grants)
            {
                logger.LogInformation($"(RemoveAll {subjectId}, {clientId}) Removing grant, key: {g.Key}");
                context.Remove(g);
            }

            await SaveChangesWithLoggerAsync(context);
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
                logger.LogInformation($"(RemoveAll {subjectId}, {clientId}, {type}) Removing grant, key: {g.Key}");
                context.Remove(g);
            }

            await SaveChangesWithLoggerAsync(context);
        }

        public async Task RemoveAsync(string key)
        {
            logger.LogInformation($"Removing grant, key: {key}");
            var grant = await context.Grants.FindAsync(key);
            if (grant != null)
            {
                context.Grants.Remove(grant);
                await SaveChangesWithLoggerAsync(context);
            }
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            if (grant == null)
            {
                return;
            }

            logger.LogInformation($"Storing grant, key: {grant.Key}");
            context.Grants.Add(grant);
            await SaveChangesWithLoggerAsync(context);
        }
    }
}