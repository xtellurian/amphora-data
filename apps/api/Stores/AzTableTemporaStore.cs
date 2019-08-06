using System.Collections.Generic;
using Amphora.Api.Contracts;
using AutoMapper;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Amphora.Api.Options;
using Amphora.Api.Models;
using Amphora.Common.Models;

namespace api.Store
{
    public class AzTableTemporaStore : IDataEntityStore<Amphora.Common.Models.Tempora>
    {
        private readonly CloudStorageAccount storageAccount;
        private readonly CloudTableClient tableClient;
        private readonly IMapper mapper;
        private readonly ILogger<AzTableAmphoraStore> logger;
        private CloudTable table;
        private bool isInit = false; // start non-initialised. Will run the first time.
        private const string tableName = "temporae";
        private const string partitionKey = "testPartition";

        public AzTableTemporaStore(IOptionsMonitor<TableStoreOptions> options,
            IMapper mapper,
            ILogger<AzTableAmphoraStore> logger)
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
        public Amphora.Common.Models.Tempora Get(string id)
        {
            InitTable();
            try
            {
                var retrieveOperation = TableOperation.Retrieve<TemporaTableEntity>(partitionKey, id);
                var result = table.Execute(retrieveOperation);
                var amphoraModelTableEntity = result.Result as TemporaTableEntity;
                if (amphoraModelTableEntity != null)
                {
                    logger.LogInformation("\t{0}\t{1}", amphoraModelTableEntity.PartitionKey, amphoraModelTableEntity.RowKey);
                }

                // Get the request units consumed by the current operation. RequestCharge of a TableResult is only applied to Azure CosmoS DB 
                if (result.RequestCharge.HasValue)
                {
                    logger.LogInformation("Request Charge of Retrieve Operation: " + result.RequestCharge);
                }

                return mapper.Map<Amphora.Common.Models.Tempora>(amphoraModelTableEntity);
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

        public Amphora.Common.Models.Tempora Set(Amphora.Common.Models.Tempora model)
        {
            InitTable();
            var entity = mapper.Map<TemporaTableEntity>(model);
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
                var insertedAmphoraModel = result.Result as TemporaTableEntity;

                // Get the request units consumed by the current operation. RequestCharge of a TableResult is only applied to Azure CosmoS DB 
                if (result.RequestCharge.HasValue)
                {
                    System.Console.WriteLine("Request Charge of InsertOrMerge Operation: " + result.RequestCharge);
                }

                return mapper.Map<Amphora.Common.Models.Tempora>(insertedAmphoraModel);
            }
            catch (StorageException e)
            {
                System.Console.WriteLine(e.Message);
                throw;
            }
        }

        public IEnumerable<Amphora.Common.Models.Tempora> List()
        {
            InitTable();
            var query = new TableQuery<TemporaTableEntity>()
            {
            };
            var queryOutput = table.ExecuteQuerySegmented<TemporaTableEntity>(query, null);

            var results = queryOutput.Results;
            
            foreach (var entity in results)
            {
                yield return mapper.Map<Amphora.Common.Models.Tempora>(entity);
            }
        }

        public Tempora Get(string id, string orgId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Tempora> List(string orgId)
        {
            throw new System.NotImplementedException();
        }
    }
}
