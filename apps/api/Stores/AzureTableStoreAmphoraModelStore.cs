
using System.Collections.Generic;
using api.Contracts;
using api.Models;
using api.Options;
using AutoMapper;
using Amphora.Common.Models;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace api.Store
{
    public class AzureTableStoreAmphoraModelService : IAmphoraEntityStore<AmphoraModel>
    {
        private readonly CloudStorageAccount storageAccount;
        private readonly CloudTableClient tableClient;
        private readonly IMapper mapper;
        private readonly ILogger<AzureTableStoreAmphoraModelService> logger;
        private CloudTable table;
        private bool isInit = false; // start non-initialised. Will run the first time.
        private const string tableName = "amphorae";
        private const string partitionKey = "testPartition";

        public AzureTableStoreAmphoraModelService(IOptionsMonitor<TableStoreOptions> options,
            IMapper mapper,
            ILogger<AzureTableStoreAmphoraModelService> logger)
        {
            this.storageAccount = CloudStorageAccount.Parse(options.CurrentValue.StorageConnectionString);
            this.tableClient = this.storageAccount.CreateCloudTableClient();
            this.mapper = mapper;
            this.logger = logger;
        }

        private void InitTable()
        {
            if (isInit && this.table != null) return;

            this.table = tableClient.GetTableReference(tableName);
            if (table.CreateIfNotExists())
            {
                System.Console.WriteLine("Created Table named: {0}", tableName);
            }
            else
            {
                System.Console.WriteLine("Table {0} already exists", tableName);
            }
            this.isInit = true;
        }
        public AmphoraModel Get(string id)
        {
            InitTable();
            try
            {
                var retrieveOperation = TableOperation.Retrieve<AmphoraModelTableEntity>(partitionKey, id);
                var result = table.Execute(retrieveOperation);
                var amphoraModelTableEntity = result.Result as AmphoraModelTableEntity;
                if (amphoraModelTableEntity != null)
                {
                    logger.LogInformation("\t{0}\t{1}", amphoraModelTableEntity.PartitionKey, amphoraModelTableEntity.RowKey);
                }

                // Get the request units consumed by the current operation. RequestCharge of a TableResult is only applied to Azure CosmoS DB 
                if (result.RequestCharge.HasValue)
                {
                    logger.LogInformation("Request Charge of Retrieve Operation: " + result.RequestCharge);
                }

                return mapper.Map<AmphoraModel>(amphoraModelTableEntity);
            }
            catch (StorageException e)
            {
                logger.LogError(e.Message);
                throw;
            }

        }

        public IEnumerable<string> ListIds()
        {
            InitTable();
            var query = new TableQuery<DynamicTableEntity>()
            {
                SelectColumns = new List<string>() { "RowKey" }
            };
            var queryOutput = table.ExecuteQuerySegmented<DynamicTableEntity>(query, null);

            var results = queryOutput.Results;
            
            foreach (var entity in results)
            {
                yield return entity.RowKey;
            }
        }

        public AmphoraModel Set(AmphoraModel model)
        {
            InitTable();
            var entity = mapper.Map<AmphoraModelTableEntity>(model);
            // set the id, rowkey, and partitionKey
            if (string.IsNullOrEmpty(entity.Id))
            {
                entity.Id = System.Guid.NewGuid().ToString();
            }
            entity.PartitionKey = partitionKey;
            entity.RowKey = entity.Id;
            
            // try the insertion
            try
            {
                // Create the InsertOrReplace table operation
                var insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                // Execute the operation.
                var result = table.Execute(insertOrMergeOperation);
                var insertedAmphoraModel = result.Result as AmphoraModelTableEntity;

                // Get the request units consumed by the current operation. RequestCharge of a TableResult is only applied to Azure CosmoS DB 
                if (result.RequestCharge.HasValue)
                {
                    System.Console.WriteLine("Request Charge of InsertOrMerge Operation: " + result.RequestCharge);
                }

                return mapper.Map<AmphoraModel>(insertedAmphoraModel);
            }
            catch (StorageException e)
            {
                System.Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
