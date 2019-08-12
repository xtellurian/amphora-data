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
    public class AzTableOrgEntityStore<T, TTableEntity> : AzTableEntityStore<T,TTableEntity>, 
        IOrgScopedEntityStore<T> where T : class, IOrgScoped where TTableEntity : class, ITableEntity, new()
    {
        public AzTableOrgEntityStore(
            IOptionsMonitor<TableStoreOptions> options,
            IOptionsMonitor<EntityTableStoreOptions<TTableEntity>> tableOptions,
            IMapper mapper,
            ILogger<AzTableOrgEntityStore<T, TTableEntity>> logger) : base(options, tableOptions, mapper, logger)
        {
        }

        public async Task<IList<T>> ListAsync(string orgId)
        {
            await InitTableAsync();

            var query = new TableQuery<TTableEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, orgId));

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
    }
}
