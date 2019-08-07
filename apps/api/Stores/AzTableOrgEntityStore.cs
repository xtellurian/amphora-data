using System.Collections.Generic;
using AutoMapper;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Amphora.Api.Options;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Api.Contracts;
using System.Linq;

namespace api.Store
{
    public class AzTableOrgEntityStore<T, TTableEntity> : IOrgEntityStore<T> where T : class, IOrgEntity where TTableEntity : class, ITableEntity, new()
    {
        private readonly CloudStorageAccount storageAccount;
        private readonly CloudTableClient tableClient;
        private readonly string tableName;
        private readonly IMapper mapper;
        private readonly ILogger<AzTableOrgEntityStore<T, TTableEntity>> logger;
        private CloudTable table;
        private bool isInit = false; // start non-initialised. Will run the first time.
        public AzTableOrgEntityStore(
            IOptionsMonitor<TableStoreOptions> options,
            IOptionsMonitor<EntityTableStoreOptions<TTableEntity>> tableOptions,
            IMapper mapper,
            ILogger<AzTableOrgEntityStore<T, TTableEntity>> logger)
        {
            if (string.IsNullOrEmpty(tableOptions.CurrentValue.TableName))
            {
                throw new System.ArgumentException("null or empty", nameof(tableOptions.CurrentValue.TableName));
            }

            this.storageAccount = CloudStorageAccount.Parse(options.CurrentValue.StorageConnectionString);
            this.tableClient = this.storageAccount.CreateCloudTableClient();
            this.tableName = tableOptions.CurrentValue.TableName;
            this.mapper = mapper;
            this.logger = logger;
        }

        private async Task InitTableAsync()
        {
            if (isInit && this.table != null) return;

            this.table = tableClient.GetTableReference(tableName);
            if (await table.CreateIfNotExistsAsync())
            {
                logger.LogWarning("Created Table named: {0}", tableName);
            }
            else
            {
                logger.LogInformation("Table {0} already exists", tableName);
            }
            this.isInit = true;
        }

        public async Task<IList<T>> ListAsync(string orgId)
        {
            await InitTableAsync();

            var query = new TableQuery<TTableEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, orgId));

            var queryOutput = table.ExecuteQuerySegmented<TTableEntity>(query, null);

            return mapper.Map<List<T>>(queryOutput.Results);

        }

        public async Task<IList<T>> ListAsync()
        {
            await InitTableAsync();
            var query = new TableQuery<TTableEntity>(); // empty query - list everything
            var queryOutput = table.ExecuteQuerySegmented<TTableEntity>(query, null);

            return mapper.Map<List<T>>(queryOutput.Results);
        }

        public async Task<T> CreateAsync(T model)
        {
            await InitTableAsync();
            if(string.IsNullOrEmpty(model.Id)) model.Id = System.Guid.NewGuid().ToString();
            var entity = mapper.Map<TTableEntity>(model);

            // try the insertion
            try
            {
                // Create the InsertOrReplace table operation
                var insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                // Execute the operation.
                var result = table.Execute(insertOrMergeOperation);
                var inserted = result.Result as TTableEntity;

                // Get the request units consumed by the current operation. RequestCharge of a TableResult is only applied to Azure CosmoS DB 
                if (result.RequestCharge.HasValue)
                {
                    System.Console.WriteLine("Request Charge of InsertOrMerge Operation: " + result.RequestCharge);
                }

                return mapper.Map<T>(inserted);
            }
            catch (StorageException e)
            {
                System.Console.WriteLine(e.Message);
                throw;
            }
        }

         public async Task<T> ReadAsync(string id)
        {
            await InitTableAsync();

            var query = new TableQuery<TTableEntity>()
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id));

            var queryOutput = table.ExecuteQuerySegmented<TTableEntity>(query, null); // TODO

            // Get the request units consumed by the current operation. RequestCharge of a TableResult is only applied to Azure CosmoS DB 
            if (queryOutput.RequestCharge.HasValue)
            {
                System.Console.WriteLine("Request Charge of InsertOrMerge Operation: " + queryOutput.RequestCharge);
            }

            if (queryOutput.Results.Count > 1)
            {
                logger.LogWarning($"{id} exists twice");
            }

            return mapper.Map<T>(queryOutput.Results.FirstOrDefault());
        }

        public async Task<T> ReadAsync(string id, string orgId = "default")
        {
            await InitTableAsync();
            try
            {
                var retrieveOperation = TableOperation.Retrieve<TTableEntity>(orgId, id);
                var result = table.Execute(retrieveOperation);
                var tableEntity = result.Result as TTableEntity;
                if (tableEntity != null)
                {
                    logger.LogInformation("\t{0}\t{1}", tableEntity.PartitionKey, tableEntity.RowKey);
                }

                // Get the request units consumed by the current operation. RequestCharge of a TableResult is only applied to Azure CosmoS DB 
                if (result.RequestCharge.HasValue)
                {
                    logger.LogInformation("Request Charge of Retrieve Operation: " + result.RequestCharge);
                }

                return mapper.Map<T>(tableEntity);
            }
            catch (StorageException e)
            {
                logger.LogError(e.Message);
                throw;
            }

        }

        public Task<T> UpdateAsync(T entity)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(T entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
