using System.Collections.Generic;
using AutoMapper;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Amphora.Api.Options;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Api.Contracts;

namespace api.Store
{
    public abstract class AzTableOrgEntityStore<T, TTableEntity> : IOrgEntityStore<T> where T: class, IOrgEntity where TTableEntity : class, ITableEntity, new()
    {
        private readonly CloudStorageAccount storageAccount;
        private readonly CloudTableClient tableClient;
        private readonly string tableName;
        private readonly IMapper mapper;
        private readonly ILogger<AzTableOrgEntityStore<T, TTableEntity>> logger;
        private CloudTable table;
        private bool isInit = false; // start non-initialised. Will run the first time.

        public AzTableOrgEntityStore(string tableName, 
            IOptionsMonitor<TableStoreOptions> options,
            IMapper mapper,
            ILogger<AzTableOrgEntityStore<T, TTableEntity>> logger)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new System.ArgumentException("null or empty", nameof(tableName));
            }

            this.storageAccount = CloudStorageAccount.Parse(options.CurrentValue.StorageConnectionString);
            this.tableClient = this.storageAccount.CreateCloudTableClient();
            this.tableName = tableName;
            this.mapper = mapper;
            this.logger = logger;
        }

        private async Task InitTableAsync()
        {
            if (isInit && this.table != null) return;

            this.table = tableClient.GetTableReference(tableName);
            if (await table.CreateIfNotExistsAsync())
            {
                System.Console.WriteLine("Created Table named: {0}", tableName);
            }
            else
            {
                System.Console.WriteLine("Table {0} already exists", tableName);
            }
            this.isInit = true;
        }
        public async Task<T> GetAsync(string id, string orgId = "default")
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

        public async Task<T> SetAsync(T model)
        {
            await InitTableAsync();
            // set the id, rowkey, and partitionKey
            if (string.IsNullOrEmpty(model.Id))
            {
                model.Id = System.Guid.NewGuid().ToString();
            }
            var entity = mapper.Map<TTableEntity>(model);
            entity.RowKey = model.Id;
            entity.PartitionKey = model.OrgId;
            
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

        public async Task<IEnumerable<T>> ListAsync(string orgId)
        {
            await InitTableAsync();
            var query = new TableQuery<TTableEntity>()
            {
            };
            var queryOutput = table.ExecuteQuerySegmented<TTableEntity>(query, null);

            return mapper.Map<List<T>>(queryOutput.Results);

        }
    }
}
