using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Stores
{
    public class CosmosAmphoraStore : IOrgScopedEntityStore<Amphora.Common.Models.Amphora>
    {
        private readonly CosmosClient client;
        private readonly Database database;
        private readonly ILogger<CosmosAmphoraStore> logger;
        private Container container;

        public CosmosAmphoraStore(IOptionsMonitor<CosmosOptions> options, ILogger<CosmosAmphoraStore> logger)
        {
            this.client = new CosmosClient(options.CurrentValue.Endpoint, options.CurrentValue.Key);
            this.database = client.GetDatabase(options.CurrentValue.Database);
            this.logger = logger;
        }

        private async Task Init()
        {
            if (this.container != null) return;
            var response = await this.database.CreateContainerIfNotExistsAsync("Amphora", "/OrgId");
            container = response.Container;
        }

        public async Task<Common.Models.Amphora> CreateAsync(Common.Models.Amphora entity)
        {
            await Init();
            // set the ids
            entity.AmphoraId =  System.Guid.NewGuid().ToString();
            entity.Id = "Amphora|" + entity.AmphoraId;
            // store the item
            var response = await this.container.CreateItemAsync(entity);
            return response.Resource;
        }

        public async Task<IList<Common.Models.Amphora>> ListAsync()
        {
            await Init();
            var results = new List<Common.Models.Amphora>(); // todo

            var x = container.GetItemQueryIterator<Common.Models.Amphora>();
            while (x.HasMoreResults)
            {
                var returned = await x.ReadNextAsync();
                results.AddRange(returned.Resource);
            }

            return results;
        }

        public async Task<Common.Models.Amphora> ReadAsync(string id)
        {
            await Init();
            try
            {
                var sqlQueryText = $"SELECT * FROM c WHERE c.id = '{id}'";
                logger.LogInformation("Running query: {0}\n", sqlQueryText);

                QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
                FeedIterator<Common.Models.Amphora> queryResultSetIterator =
                    this.container.GetItemQueryIterator<Common.Models.Amphora>(queryDefinition);

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
                return entities.FirstOrDefault();;
            }
            catch (System.Exception ex)
            {
                logger.LogError("Error Retrieving Entity", ex);
                throw ex;
            }
        }

        public async Task<Common.Models.Amphora> UpdateAsync(Common.Models.Amphora entity)
        {
            await Init();
            var response = await container.UpsertItemAsync(entity);
            return response.Resource;
        }

        public async Task DeleteAsync(Common.Models.Amphora entity)
        {
            await Init();
            await container.DeleteItemAsync<Common.Models.Amphora>(entity.Id, new PartitionKey(entity.OrgId));
        }

        public async Task<IList<Common.Models.Amphora>> StartsWithQueryAsync(string propertyName, string givenValue)
        {
            await Init();
            var sqlQueryText = $"SELECT * FROM c WHERE STARTSWITH(c.{propertyName}, '{givenValue}')";
            logger.LogInformation("Running query: {0}\n", sqlQueryText);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Common.Models.Amphora> queryResultSetIterator =
                this.container.GetItemQueryIterator<Common.Models.Amphora>(queryDefinition);

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
            var response = await container.ReadItemAsync<Common.Models.Amphora>(id, new PartitionKey(orgId));
            return response.Resource;
        }

        public async Task<IList<Common.Models.Amphora>> ListAsync(string orgId)
        {
            await Init();
            return container.GetItemLinqQueryable<Common.Models.Amphora>()
                .Where(a => a.OrgId == orgId)
                .ToList(); // TODO - performance
        }
    }
}