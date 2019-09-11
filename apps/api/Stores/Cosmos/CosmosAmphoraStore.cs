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
    public class CosmosAmphoraStore : CosmosStoreBase, IEntityStore<Amphora.Common.Models.AmphoraModel>
    {

        public CosmosAmphoraStore(IOptionsMonitor<CosmosOptions> options, ILogger<CosmosAmphoraStore> logger)
            : base(options, logger, "Amphora", "/OrganisationId")
        {
        }

        public async Task<Common.Models.AmphoraModel> CreateAsync(Common.Models.AmphoraModel entity)
        {
            return await base.CreateAsync<Common.Models.AmphoraModel>(entity);
        }

        public async Task<IList<Common.Models.AmphoraModel>> ListAsync()
        {
            await Init();
            return await base.ListAsync<Common.Models.AmphoraModel>();
        }

        public async Task<Common.Models.AmphoraModel> ReadAsync(string id)
        {
            await Init();
            if (id == null) return null;
            id = id.AsQualifiedId(typeof(Common.Models.AmphoraModel));
            return await ReadAsync<Common.Models.AmphoraModel>(id);
        }

        public async Task<Common.Models.AmphoraModel> UpdateAsync(Common.Models.AmphoraModel entity)
        {
            await Init();
            return await this.UpdateAsync<Common.Models.AmphoraModel>(entity);
        }

        public async Task DeleteAsync(Common.Models.AmphoraModel entity)
        {
            await Init();
            await this.DeleteAsync<Common.Models.AmphoraModel>(entity);
        }

        public async Task<IList<Common.Models.AmphoraModel>> StartsWithQueryAsync(string propertyName, string givenValue)
        {
            await Init();
            var sqlQueryText = $"SELECT * FROM c WHERE STARTSWITH(c.{propertyName}, '{givenValue}')";
            logger.LogInformation("Running query: {0}\n", sqlQueryText);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Common.Models.AmphoraModel> queryResultSetIterator =
                this.Container.GetItemQueryIterator<Common.Models.AmphoraModel>(queryDefinition);

            var entities = new List<Common.Models.AmphoraModel>();

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

        public async Task<Common.Models.AmphoraModel> ReadAsync(string id, string orgId)
        {
            await Init();
            id = id.AsQualifiedId(typeof(Common.Models.AmphoraModel));
            if (orgId == null) return await this.ReadAsync(id);
            else
            {
                var response = await Container.ReadItemAsync<Common.Models.AmphoraModel>(id, new PartitionKey(orgId));
                return response.Resource;
            }
        }

        public async Task<IList<Common.Models.AmphoraModel>> ListAsync(string orgId)
        {
            await Init();
            return Container.GetItemLinqQueryable<Common.Models.AmphoraModel>()
                .Where(a => a.OrganisationId == orgId)
                .ToList(); // TODO - performance
        }

        public Task<IEnumerable<Common.Models.AmphoraModel>> QueryAsync(Func<Common.Models.AmphoraModel, bool> where)
        {
            return base.QueryAsync(where);
        }
    }
}