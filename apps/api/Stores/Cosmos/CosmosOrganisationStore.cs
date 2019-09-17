using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Common.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System;
using Amphora.Common.Models.Organisations;

namespace Amphora.Api.Stores.Cosmos
{
    public class CosmosOrganisationStore : CosmosStoreBase, IEntityStore<OrganisationModel>
    {

        public CosmosOrganisationStore(IOptionsMonitor<CosmosOptions> options, ILogger<CosmosOrganisationStore> logger)
            : base(options, logger)
        {
        }

        public async Task<OrganisationModel> CreateAsync(OrganisationModel entity)
        {
            return await base.CreateAsync(entity);
        }
        async Task<TExtended> IEntityStore<OrganisationModel>.CreateAsync<TExtended>(TExtended entity)
        {
            return await base.CreateAsync<TExtended>(entity);
        }

        public async Task DeleteAsync(OrganisationModel entity)
        {
            await Init();
            var deleteResponse = await DeleteAsync<OrganisationModel>(entity);
        }

        public async Task<IList<OrganisationModel>> TopAsync()
        {
            await Init();
            return await base.TopAsync<OrganisationModel>();
        }

        public async Task<IList<OrganisationModel>> TopAsync(string orgId)
        {
            await Init();
            return Container.GetItemLinqQueryable<OrganisationModel>()
                .Where(a => a.OrganisationId == orgId)
                .ToList(); // TODO - performance
        }

        public async Task<OrganisationModel> ReadAsync(string id)
        {
            await Init();
            if (id == null) return null;
            id = id.AsQualifiedId(typeof(OrganisationModel));
            return await base.ReadAsync<OrganisationModel>(id);
        }
        public async Task<OrganisationModel> ReadAsync(string id, string orgId)
        {
            await Init();
            if (id == null) return null;
            id = id.AsQualifiedId(typeof(OrganisationModel));
            return await base.ReadAsync<OrganisationModel>(id, orgId);
        }
        async Task<TExtended> IEntityStore<OrganisationModel>.ReadAsync<TExtended>(string id)
        {
            await Init();
            id = id.AsQualifiedId<OrganisationModel>();
            return await base.ReadAsync<TExtended>(id);
        }
        async Task<TExtended> IEntityStore<OrganisationModel>.ReadAsync<TExtended>(string id, string orgId)
        {
            await Init();
            id = id.AsQualifiedId<OrganisationModel>();
            return await base.ReadAsync<TExtended>(id, orgId);
        }


        Task<IList<TExtended>> IEntityStore<OrganisationModel>.StartsWithQueryAsync<TExtended>(string propertyName, string givenValue)
        {
            throw new System.NotImplementedException();
        }

        public async Task<OrganisationModel> UpdateAsync(OrganisationModel entity)
        {
            await Init();
            return await base.UpdateAsync<OrganisationModel>(entity);
        }

        async Task<IEnumerable<TQuery>> IEntityStore<OrganisationModel>.QueryAsync<TQuery>(Func<TQuery, bool> where)
        {
            return await base.QueryAsync(where);
        }
    }
}