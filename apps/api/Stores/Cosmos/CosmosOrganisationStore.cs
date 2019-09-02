using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Common.Models;
using Amphora.Common.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;

namespace Amphora.Api.Stores.Cosmos
{
    public class CosmosOrganisationStore : CosmosStoreBase, IEntityStore<Organisation>
    {

        public CosmosOrganisationStore(IOptionsMonitor<CosmosOptions> options, ILogger<CosmosOrganisationStore> logger) 
            : base(options, logger)
        {
        }

        public async Task<Organisation> CreateAsync(Organisation entity)
        {
            await Init();
            // set the ids
            entity.OrganisationId =  System.Guid.NewGuid().ToString();
            entity.Id = "Organisation|" + entity.OrganisationId;
            // store the item
            var response = await this.Container.CreateItemAsync(entity);
            return response.Resource;
        }

        public async Task DeleteAsync(Organisation entity)
        {
            await Init();
            var deleteResponse = await DeleteAsync<Organisation>(entity);
        }

        public async Task<IList<Organisation>> ListAsync()
        {
            await Init();
            return await base.ListAsync<Organisation>();
        }

        public async Task<IList<Organisation>> ListAsync(string orgId)
        {
            await Init();
            return Container.GetItemLinqQueryable<Organisation>()
                .Where(a => a.OrganisationId == orgId)
                .ToList(); // TODO - performance
        }

        public async Task<Organisation> ReadAsync(string id)
        {
            await Init();
            if (id == null) return null;
            id = id.AsQualifiedId(typeof(Organisation));
            return await this.ReadAsync<Organisation>(id);
        }

        public Task<Organisation> ReadAsync(string id, string orgId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IList<Organisation>> StartsWithQueryAsync(string propertyName, string givenValue)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Organisation> UpdateAsync(Organisation entity)
        {
            await Init();
            return await base.UpdateAsync<Organisation>(entity);
        }
    }
}