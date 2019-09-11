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

        public async Task DeleteAsync(OrganisationModel entity)
        {
            await Init();
            var deleteResponse = await DeleteAsync<OrganisationModel>(entity);
        }

        public async Task<IList<OrganisationModel>> ListAsync()
        {
            await Init();
            return await base.ListAsync<OrganisationModel>();
        }

        public async Task<IList<OrganisationModel>> ListAsync(string orgId)
        {
            await Init();
            return Container.GetItemLinqQueryable<OrganisationModel>()
                .Where(a => a.OrganisationId == orgId)
                .ToList(); // TODO - performance
        }

        public Task<IEnumerable<OrganisationModel>> QueryAsync(Func<OrganisationModel, bool> where)
        {
            return base.QueryAsync(where);
        }

        public async Task<OrganisationModel> ReadAsync(string id)
        {
            await Init();
            if (id == null) return null;
            id = id.AsQualifiedId(typeof(OrganisationModel));
            return await base.ReadAsync<OrganisationModel>(id);
        }

        Task<TExtended> IEntityStore<OrganisationModel>.ReadAsync<TExtended>(string id, string orgId)
        {
            throw new NotImplementedException();
        }

        public async Task<OrganisationModel> ReadAsync(string id, string orgId)
        {
            await Init();
            if(id == null) return null;
            id = id.AsQualifiedId(typeof(OrganisationModel));
            return await base.ReadAsync<OrganisationModel>(id, orgId);
        }

        public Task<IList<OrganisationModel>> StartsWithQueryAsync(string propertyName, string givenValue)
        {
            throw new System.NotImplementedException();
        }

        public async Task<OrganisationModel> UpdateAsync(OrganisationModel entity)
        {
            await Init();
            return await base.UpdateAsync<OrganisationModel>(entity);
        }
    }
}