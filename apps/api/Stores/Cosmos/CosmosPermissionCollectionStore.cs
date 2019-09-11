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
    public class CosmosPermissionCollectionStore : CosmosStoreBase, IEntityStore<PermissionModel>
    {
        public CosmosPermissionCollectionStore(IOptionsMonitor<CosmosOptions> options, ILogger<CosmosStoreBase> logger)
            : base(options, logger)
        {
        }

        public async Task<PermissionModel> CreateAsync(PermissionModel entity)
        {
            return await base.CreateAsync(entity);
        }

        public Task DeleteAsync(PermissionModel entity)
        {
            throw new NotImplementedException();
        }

        public Task<IList<PermissionModel>> ListAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IList<PermissionModel>> ListAsync(string orgId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PermissionModel>> QueryAsync(Func<PermissionModel, bool> where)
        {
            await Init();
            return this.Container.GetItemLinqQueryable<PermissionModel>(true).Where(where);
        }

        public async Task<PermissionModel> ReadAsync(string id)
        {
            try
            {
                return await base.ReadAsync<PermissionModel>(id);
            }
            catch(Exception ex)
            {
                logger.LogError($"ReadAsync throws with id= {id}", ex);
                return null;
            }
        }

        public async Task<PermissionModel> ReadAsync(string id, string orgId)
        {
            try
            {
                var response = await base.ReadAsync<PermissionModel>(id, orgId);
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"ReadAsync throws with id= {id} and orgId = {orgId}", ex);
                return null;
            }
        }

        public Task<IList<PermissionModel>> StartsWithQueryAsync(string propertyName, string givenValue)
        {
            throw new NotImplementedException();
        }

        public async Task<PermissionModel> UpdateAsync(PermissionModel entity)
        {
            return await base.UpdateAsync(entity);
        }
    }
}