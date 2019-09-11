using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Common.Extensions;
using Amphora.Common.Models;
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

        public async Task<AmphoraModel> CreateAsync(AmphoraModel entity)
        {
            return await base.CreateAsync<AmphoraModel>(entity);
        }

        public async Task<IList<AmphoraModel>> ListAsync()
        {
            await Init();
            return await base.ListAsync<AmphoraModel>();
        }

        public async Task<AmphoraModel> ReadAsync(string id)
        {
            await Init();
            if (id == null) return null;
            id = id.AsQualifiedId(typeof(AmphoraModel));
            return await ReadAsync<AmphoraModel>(id);
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

        public async Task<IList<Common.Models.AmphoraModel>> StartsWithQueryAsync(string propertyName, string givenValue)
        {
            await Init();
            var sqlQueryText = $"SELECT * FROM c WHERE STARTSWITH(c.{propertyName}, '{givenValue}')";
            logger.LogInformation("Running query: {0}\n", sqlQueryText);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<AmphoraModel> queryResultSetIterator =
                this.Container.GetItemQueryIterator<AmphoraModel>(queryDefinition);

            var entities = new List<AmphoraModel>();

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
        Task<TExtended> IEntityStore<AmphoraModel>.ReadAsync<TExtended>(string id, string orgId)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<AmphoraModel>> ListAsync(string orgId)
        {
            await Init();
            return Container.GetItemLinqQueryable<AmphoraModel>()
                .Where(a => a.OrganisationId == orgId)
                .ToList(); // TODO - performance
        }

        public Task<IEnumerable<AmphoraModel>> QueryAsync(Func<AmphoraModel, bool> where)
        {
            return base.QueryAsync(where);
        }
    }
}