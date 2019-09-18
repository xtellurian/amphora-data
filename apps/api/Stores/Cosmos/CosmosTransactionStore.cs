using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Options;
using Amphora.Common.Models.Transactions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Amphora.Common.Extensions;

namespace Amphora.Api.Stores.Cosmos
{
    public class CosmosTransactionStore : CosmosStoreBase, IEntityStore<TransactionModel>
    {
        public CosmosTransactionStore(IOptionsMonitor<CosmosOptions> options, ILogger<CosmosTransactionStore> logger)
            : base(options, logger, "Transactions") // use a different container
        {

        }

        public async Task<TransactionModel> CreateAsync(TransactionModel entity)
        {
            await Init();
            return await base.CreateAsync(entity);

        }
        async Task<TExtended> IEntityStore<TransactionModel>.CreateAsync<TExtended>(TExtended entity)
        {
            await Init();
            return await base.CreateAsync<TExtended>(entity);
        }
        public async Task<TransactionModel> ReadAsync(string id)
        {
            await Init();
            if (string.IsNullOrEmpty(id)) return null;
            id = id.AsQualifiedId<TransactionModel>();
            return await base.ReadAsync<TransactionModel>(id);
        }

        public async Task<TransactionModel> ReadAsync(string id, string orgId)
        {
            await Init();
            if (string.IsNullOrEmpty(id)) return null;
            id = id.AsQualifiedId<TransactionModel>();
            return await base.ReadAsync<TransactionModel>(id, orgId);
        }
        async Task<TExtended> IEntityStore<TransactionModel>.ReadAsync<TExtended>(string id)
        {
            await Init();
            if (string.IsNullOrEmpty(id)) return null;
            id = id.AsQualifiedId<TransactionModel>();
            return await base.ReadAsync<TExtended>(id);
        }
        async Task<TExtended> IEntityStore<TransactionModel>.ReadAsync<TExtended>(string id, string orgId)
        {
            await Init();
             if (string.IsNullOrEmpty(id)) return null;
            id = id.AsQualifiedId<TransactionModel>();
            if(string.IsNullOrEmpty(orgId)) return await base.ReadAsync<TExtended>(id);
            else return await base.ReadAsync<TExtended>(id, orgId); 
        }
        async Task IEntityStore<TransactionModel>.DeleteAsync(TransactionModel entity)
        {
            await Init();
            await base.DeleteAsync(entity);
        }

        public Task<IList<TransactionModel>> TopAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IList<TransactionModel>> TopAsync(string orgId)
        {
            throw new NotImplementedException();
        }

        public async Task<TransactionModel> UpdateAsync(TransactionModel entity)
        {
            await Init();
            return await base.UpdateAsync(entity);
        }

        async Task<IEnumerable<TQuery>> IEntityStore<TransactionModel>.QueryAsync<TQuery>(Func<TQuery, bool> where)
        {
            await Init();
            return await base.QueryAsync(where);
        }
    }
}