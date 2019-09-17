using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Common.Models.Permissions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Amphora.Common.Extensions;

namespace Amphora.Api.Stores.Cosmos
{
    public class CosmosPermissionStore : CosmosStoreBase, IEntityStore<PermissionModel>
    {
        public CosmosPermissionStore(IOptionsMonitor<CosmosOptions> options, ILogger<CosmosStoreBase> logger)
            : base(options, logger)
        {
        }

        public async Task<PermissionModel> CreateAsync(PermissionModel entity)
        {
            return await base.CreateAsync(entity);
        }
        async Task<TExtended> IEntityStore<PermissionModel>.CreateAsync<TExtended>(TExtended entity)
        {
            return await base.CreateAsync<TExtended>(entity);
        }

        public Task DeleteAsync(PermissionModel entity)
        {
            throw new NotImplementedException();
        }

        public Task<IList<PermissionModel>> TopAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IList<PermissionModel>> TopAsync(string orgId)
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

        Task<TExtended> IEntityStore<PermissionModel>.ReadAsync<TExtended>(string id, string orgId)
        {
            id = id.AsQualifiedId<PermissionModel>();
            return base.ReadAsync<TExtended>(id, orgId);
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

        Task<IList<TExtended>> IEntityStore<PermissionModel>.StartsWithQueryAsync<TExtended>(string propertyName, string givenValue)
        {
            throw new NotImplementedException();
        }

        public async Task<PermissionModel> UpdateAsync(PermissionModel entity)
        {
            return await base.UpdateAsync(entity);
        }

        async Task<IEnumerable<TQuery>> IEntityStore<PermissionModel>.QueryAsync<TQuery>(Func<TQuery, bool> where)
        {
            await Init();
            return await base.QueryAsync(where);
        }

        async Task<TExtended> IEntityStore<PermissionModel>.ReadAsync<TExtended>(string id)
        {
            await Init();
            id = id.AsQualifiedId<PermissionModel>();
            return await base.ReadAsync<TExtended>(id);
        }
    }
}