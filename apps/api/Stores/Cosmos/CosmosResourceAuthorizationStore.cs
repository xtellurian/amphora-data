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
    public class CosmosResourceAuthorizationStore : CosmosStoreBase, IEntityStore<ResourceAuthorization>
    {
        public CosmosResourceAuthorizationStore(IOptionsMonitor<CosmosOptions> options, ILogger<CosmosStoreBase> logger)
            : base(options, logger)
        {
        }

        public async Task<ResourceAuthorization> CreateAsync(ResourceAuthorization entity)
        {
            return await base.CreateAsync(entity);
        }

        public Task DeleteAsync(ResourceAuthorization entity)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ResourceAuthorization>> ListAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IList<ResourceAuthorization>> ListAsync(string orgId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ResourceAuthorization>> QueryAsync(Func<ResourceAuthorization, bool> where)
        {
            await Init();
            return this.Container.GetItemLinqQueryable<ResourceAuthorization>(true).Where(where);
        }

        public async Task<ResourceAuthorization> ReadAsync(string id)
        {
            return await base.ReadAsync<ResourceAuthorization>(id);
        }

        public Task<ResourceAuthorization> ReadAsync(string id, string orgId)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ResourceAuthorization>> StartsWithQueryAsync(string propertyName, string givenValue)
        {
            throw new NotImplementedException();
        }

        public Task<ResourceAuthorization> UpdateAsync(ResourceAuthorization entity)
        {
            throw new NotImplementedException();
        }
    }
}