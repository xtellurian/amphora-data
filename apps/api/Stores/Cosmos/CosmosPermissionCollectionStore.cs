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
            try
            {
                return await base.ReadAsync<PermissionCollection>(id);
            }
            catch(Exception ex)
            {
                logger.LogError($"ReadAsync throws with id= {id}", ex);
                return null;
            }
        }

        public async Task<PermissionCollection> ReadAsync(string id, string orgId)
        {
            try
            {
                var response = await base.ReadAsync<PermissionCollection>(id, orgId);
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"ReadAsync throws with id= {id} and orgId = {orgId}", ex);
                return null;
            }
        }

        public Task<IList<PermissionCollection>> StartsWithQueryAsync(string propertyName, string givenValue)
        {
            throw new NotImplementedException();
        }

        public async Task<PermissionCollection> UpdateAsync(PermissionCollection entity)
        {
            return await base.UpdateAsync(entity);
        }
    }
}