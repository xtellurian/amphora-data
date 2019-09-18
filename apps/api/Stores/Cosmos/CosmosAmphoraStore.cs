using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Amphorae;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Stores.Cosmos
{
    public class CosmosAmphoraStore : CosmosStoreBase, IEntityStore<AmphoraModel>
    {

        public CosmosAmphoraStore(IOptionsMonitor<CosmosOptions> options, ILogger<CosmosAmphoraStore> logger)
            : base(options, logger, "Amphora", "/OrganisationId")
        {
        }

        public async Task<AmphoraModel> CreateAsync(AmphoraModel entity)
        {
            return await base.CreateAsync<AmphoraModel>(entity);
        }
        async Task<TExtended> IEntityStore<AmphoraModel>.CreateAsync<TExtended>(TExtended entity)
        {
            return await base.CreateAsync<TExtended>(entity);
        }
        public async Task<IList<AmphoraModel>> TopAsync(string orgId)
        {
            await Init();
            return Container.GetItemLinqQueryable<AmphoraModel>()
                .Where(a => a.OrganisationId == orgId)
                .ToList(); // TODO - performance
        }
        public async Task<IList<AmphoraModel>> TopAsync()
        {
            await Init();
            return await base.TopAsync<AmphoraModel>();
        }

        public async Task<AmphoraModel> ReadAsync(string id)
        {
            await Init();
            if (id == null) return null;
            id = id.AsQualifiedId(typeof(AmphoraModel));
            return await ReadAsync<AmphoraModel>(id);
        }
        public async Task<AmphoraModel> ReadAsync(string id, string orgId)
        {
            await Init();
            id = id.AsQualifiedId(typeof(AmphoraModel));
            if (orgId == null) return await this.ReadAsync(id);
            else
            {
                var response = await Container.ReadItemAsync<AmphoraModel>(id, new PartitionKey(orgId));
                return response.Resource;
            }
        }
        async Task<TExtended> IEntityStore<AmphoraModel>.ReadAsync<TExtended>(string id, string orgId)
        {
            await Init();
            id = id.AsQualifiedId<AmphoraModel>();
            if(orgId == null)
            {
                return await base.ReadAsync<TExtended>(id);
            }
            else
            {
                return await base.ReadAsync<TExtended>(id, orgId);
            }
        }
        async Task<TExtended> IEntityStore<AmphoraModel>.ReadAsync<TExtended>(string id)
        {
            await Init();
            id = id.AsQualifiedId<AmphoraModel>();
            return await base.ReadAsync<TExtended>(id);
        }

        public async Task<AmphoraModel> UpdateAsync(AmphoraModel entity)
        {
            await Init();
            return await this.UpdateAsync<AmphoraModel>(entity);
        }

        public async Task DeleteAsync(AmphoraModel entity)
        {
            await Init();
            await this.DeleteAsync<AmphoraModel>(entity);
        }

        async Task<IEnumerable<TQuery>> IEntityStore<AmphoraModel>.QueryAsync<TQuery>(Func<TQuery, bool> where)
        {
            Func<TQuery, bool> where2 = (t) => where(t) && t.EntityType == typeof(AmphoraModel).GetEntityPrefix();
            return await base.QueryAsync<TQuery>(where2);
        }
    }
}