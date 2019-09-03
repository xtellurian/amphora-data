using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Options;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Stores.Cosmos
{
    public abstract class CosmosStoreBase
    {
        protected readonly CosmosClient Client;
        protected readonly Database Database;
        protected readonly ILogger<CosmosStoreBase> logger;
        protected Container Container;

        public CosmosStoreBase(IOptionsMonitor<CosmosOptions> options,
                               ILogger<CosmosStoreBase> logger,
                               string containerName = "Amphora",
                               string partitionKey = "/OrganisationId")
        // partitionKey cannot be changed on an existing Cosmos database
        // be very careful changing the partitionKey without changing the containerName
        {
            this.Client = new CosmosClient(options.CurrentValue.Endpoint, options.CurrentValue.Key);
            this.Database = Client.GetDatabase(options.CurrentValue.Database);
            this.logger = logger;
            ContainerName = containerName;
            PartitionKey = partitionKey;
        }

        public string ContainerName { get; }
        public string PartitionKey { get; }

        protected virtual async Task Init()
        {
            if (this.Container != null) return;
            var response = await this.Database.CreateContainerIfNotExistsAsync(ContainerName, PartitionKey);
            Container = response.Container;
        }

        protected async Task<T> ReadAsync<T>(string id)
        {
            await Init();
            try
            {
                var sqlQueryText = $"SELECT * FROM c WHERE c.id = '{id}'";
                logger.LogInformation("Running query: {0}\n", sqlQueryText);

                QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
                FeedIterator<T> queryResultSetIterator =
                    this.Container.GetItemQueryIterator<T>(queryDefinition);

                var entities = new List<T>();

                while (queryResultSetIterator.HasMoreResults)
                {
                    var currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (var entity in currentResultSet)
                    {
                        entities.Add(entity);
                        logger.LogInformation("\tRead {0}\n", entity);
                    }
                }
                return entities.FirstOrDefault(); ;
            }
            catch (System.Exception ex)
            {
                logger.LogError("Error Retrieving Entity", ex);
                throw ex;
            }
        }

        protected async Task<ItemResponse<T>> DeleteAsync<T>(T entity) where T : IEntity
        {
            await Init();
            return await Container.DeleteItemAsync<T>(entity.Id, new PartitionKey(entity.OrganisationId));
        }

        protected async Task<T> UpdateAsync<T>(T entity)
        {
            await Init();
            var response = await Container.UpsertItemAsync(entity);
            return response.Resource;
        }

        protected async Task<List<T>> ListAsync<T>()
        {
            var results = new List<T>(); // todo
            var prefix = typeof(T).GetEntityPrefix();
            var queryText = $"SELECT * FROM c WHERE STARTSWITH(c.id, '{prefix}')";
            var x = Container.GetItemQueryIterator<T>(queryText);
            while (x.HasMoreResults)
            {
                var returned = await x.ReadNextAsync();
                results.AddRange(returned.Resource);
            }

            return results;
        }
    }
}