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
using Amphora.Api.Models.Queries;
using System.Text;
using System;

namespace Amphora.Api.Stores
{
    public class AzTableEntityStore<T, TTableEntity> : IEntityStore<T> where T : class, IEntity where TTableEntity : class, ITableEntity, new()
    {
        private readonly CloudStorageAccount storageAccount;
        private readonly CloudTableClient tableClient;
        protected string tableName;
        protected IMapper mapper;
        protected ILogger<AzTableEntityStore<T, TTableEntity>> logger;
        protected CloudTable table;
        private bool isInit = false; // start non-initialised. Will run the first time.
        public AzTableEntityStore(
            IOptionsMonitor<AzureStorageAccountOptions> options,
            IOptionsMonitor<EntityTableStoreOptions<TTableEntity>> tableOptions,
            IMapper mapper,
            ILogger<AzTableEntityStore<T, TTableEntity>> logger)
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

        protected async Task InitTableAsync()
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

        public virtual async Task<IList<T>> ListAsync()
        {
            await InitTableAsync();
            var query = new TableQuery<TTableEntity>(); // empty query - list everything
            var queryOutput = table.ExecuteQuerySegmented<TTableEntity>(query, null);

            return mapper.Map<List<T>>(queryOutput.Results);
        }

        public virtual async Task<T> CreateAsync(T model)
        {
            await InitTableAsync();
            model.Id = System.Guid.NewGuid().ToString();
            var entity = mapper.Map<TTableEntity>(model);

            // try the insertion
            try
            {
                // Create the InsertOrReplace table operation
                var insertOrMergeOperation = TableOperation.Insert(entity);

                // Execute the operation.
                var result = await table.ExecuteAsync(insertOrMergeOperation);
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

        public virtual async Task<T> ReadAsync(string id)
        {
            await InitTableAsync();

            var query = new TableQuery<TTableEntity>()
                .Where(TableQuery.GenerateFilterCondition("RowKey", Microsoft.Azure.Cosmos.Table.QueryComparisons.Equal, id));

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

        public virtual async Task<T> UpdateAsync(T model)
        {
            var entity = mapper.Map<TTableEntity>(model);
            if (string.IsNullOrEmpty(entity.ETag))
            {
                //TODO: ETag cache per session
                logger.LogWarning("ETag is null. Setting as wildcard *");
                entity.ETag = "*";
            }
            try
            {
                // Create the InsertOrReplace table operation
                var insertOrMergeOperation = TableOperation.Merge(entity);

                // Execute the operation.
                var result = await table.ExecuteAsync(insertOrMergeOperation);
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

        public virtual async Task DeleteAsync(T entity)
        {
            var tableEntity = mapper.Map<TTableEntity>(entity);
            if( tableEntity.ETag == null )
            {
                tableEntity.ETag = "*";
            }
            TableOperation deleteOperation = TableOperation.Delete(tableEntity);
            TableResult result = await table.ExecuteAsync(deleteOperation);
        }

        public async Task<IList<T>> StartsWithQueryAsync(string propertyName, string givenValue)
        {
            await InitTableAsync();
            var tq = new TableQuery<TTableEntity>().Where(GetStartsWithFilter(propertyName, givenValue));

            TableContinuationToken token = null;
            var entities = new List<TTableEntity>();
            do
            {
                var segment = await table.ExecuteQuerySegmentedAsync(tq, token);
                entities.AddRange(segment.Results);
                token = segment.ContinuationToken;
            }
            while (token != null);

            return mapper.Map<List<T>>(entities);

        }

        private static string GetStartsWithFilter(string columnName, string startsWith)
        {
            var length = startsWith.Length - 1;
            var nextChar = startsWith[length] + 1;

            var startWithEnd = startsWith.Substring(0, length) + (char)nextChar;
            var filter = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition(columnName, Microsoft.Azure.Cosmos.Table.QueryComparisons.GreaterThanOrEqual, startsWith),
                TableOperators.And,
                TableQuery.GenerateFilterCondition(columnName, Microsoft.Azure.Cosmos.Table.QueryComparisons.LessThan, startWithEnd));

            return filter;
        }

      public async Task<IList<T>> ListAsync(string orgId)
        {
            await InitTableAsync();

            var query = new TableQuery<TTableEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", Microsoft.Azure.Cosmos.Table.QueryComparisons.Equal, orgId));

            var queryOutput = table.ExecuteQuerySegmented<TTableEntity>(query, null);

            return mapper.Map<List<T>>(queryOutput.Results);

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

        public Task<IEnumerable<T>> QueryAsync(Func<T, bool> where)
        {
            throw new NotImplementedException();
        }
    }
}
