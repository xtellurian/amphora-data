using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Common.Extensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Stores.Cosmos
{
    public class CosmosAmphoraStore : CosmosStoreBase, IEntityStore<Amphora.Common.Models.Amphora>
    {

        public CosmosAmphoraStore(IOptionsMonitor<CosmosOptions> options, ILogger<CosmosAmphoraStore> logger)
            : base(options, logger, "Amphora", "/OrganisationId")
        {
        }

        public async Task<Common.Models.Amphora> CreateAsync(Common.Models.Amphora entity)
        {
            return await base.CreateAsync<Common.Models.Amphora>(entity);
        }

        public async Task<IList<Common.Models.Amphora>> ListAsync()
        {
            await Init();
            return await base.ListAsync<Common.Models.Amphora>();
        }

        public async Task<Common.Models.Amphora> ReadAsync(string id)
        {
            await Init();
            if (id == null) return null;
            id = id.AsQualifiedId(typeof(Common.Models.Amphora));
            return await ReadAsync<Common.Models.Amphora>(id);
        }

        public async Task<Common.Models.Amphora> UpdateAsync(Common.Models.Amphora entity)
        {
            await Init();
            return await this.UpdateAsync<Common.Models.Amphora>(entity);
        }

        public async Task DeleteAsync(Common.Models.Amphora entity)
        {
            await Init();
            await this.DeleteAsync<Common.Models.Amphora>(entity);
        }

        public async Task<IList<Common.Models.Amphora>> StartsWithQueryAsync(string propertyName, string givenValue)
        {
            await Init();
            var sqlQueryText = $"SELECT * FROM c WHERE STARTSWITH(c.{propertyName}, '{givenValue}')";
            logger.LogInformation("Running query: {0}\n", sqlQueryText);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Common.Models.Amphora> queryResultSetIterator =
                this.Container.GetItemQueryIterator<Common.Models.Amphora>(queryDefinition);

            var entities = new List<Common.Models.Amphora>();

            while (queryResultSetIterator.HasMoreResults)
            {
                var currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (var entity in currentResultSet)
                {
                    entities.Add(entity);
                    logger.LogInformation("\tRead {0}\n", entity);
                }
            }
            return entities;
        }

        public async Task<Common.Models.Amphora> ReadAsync(string id, string orgId)
        {
            await Init();
            id = id.AsQualifiedId(typeof(Common.Models.Amphora));
            if (orgId == null) return await this.ReadAsync(id);
            else
            {
                var response = await Container.ReadItemAsync<Common.Models.Amphora>(id, new PartitionKey(orgId));
                return response.Resource;
            }
        }

        public async Task<IList<Common.Models.Amphora>> ListAsync(string orgId)
        {
            await Init();
            return Container.GetItemLinqQueryable<Common.Models.Amphora>()
                .Where(a => a.OrganisationId == orgId)
                .ToList(); // TODO - performance
        }

        public Task<IEnumerable<Common.Models.Amphora>> QueryAsync(Func<Common.Models.Amphora, bool> where)
        {
            return base.QueryAsync(where);
        }
    }
}