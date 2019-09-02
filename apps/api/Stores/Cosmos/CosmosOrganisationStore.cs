using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Common.Models;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Stores.Cosmos
{
    public class CosmosOrganisationStore : CosmosStoreBase, IEntityStore<Organisation>
    {
        public CosmosOrganisationStore(IOptionsMonitor<CosmosOptions> options) 
            : base(options)
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

        public Task DeleteAsync(Organisation entity)
        {
            throw new System.NotImplementedException();
        }

        public Task<IList<Organisation>> ListAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<IList<Organisation>> ListAsync(string orgId)
        {
            throw new System.NotImplementedException();
        }

        public Task<Organisation> ReadAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<Organisation> ReadAsync(string id, string orgId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IList<Organisation>> StartsWithQueryAsync(string propertyName, string givenValue)
        {
            throw new System.NotImplementedException();
        }

        public Task<Organisation> UpdateAsync(Organisation entity)
        {
            throw new System.NotImplementedException();
        }
    }
}