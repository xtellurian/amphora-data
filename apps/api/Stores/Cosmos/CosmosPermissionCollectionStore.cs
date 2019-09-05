using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Stores.Cosmos
{
    public class CosmosPermissionCollectionStore : CosmosStoreBase, IEntityStore<PermissionCollection>
    {
        public CosmosPermissionCollectionStore(IOptionsMonitor<CosmosOptions> options, ILogger<CosmosStoreBase> logger)
            : base(options, logger)
        {
        }

        public async Task<PermissionCollection> CreateAsync(PermissionCollection entity)
        {
            return await base.CreateAsync(entity);
        }

        public Task DeleteAsync(PermissionCollection entity)
        {
            throw new NotImplementedException();
        }

        public Task<IList<PermissionCollection>> ListAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IList<PermissionCollection>> ListAsync(string orgId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PermissionCollection>> QueryAsync(Func<PermissionCollection, bool> where)
        {
            await Init();
            return this.Container.GetItemLinqQueryable<PermissionCollection>(true).Where(where);
        }

        public async Task<PermissionCollection> ReadAsync(string id)
        {
            return await base.ReadAsync<PermissionCollection>(id);
        }

        public Task<PermissionCollection> ReadAsync(string id, string orgId)
        {
            throw new NotImplementedException();
        }

        public Task<IList<PermissionCollection>> StartsWithQueryAsync(string propertyName, string givenValue)
        {
            throw new NotImplementedException();
        }

        public Task<PermissionCollection> UpdateAsync(PermissionCollection entity)
        {
            throw new NotImplementedException();
        }
    }
}