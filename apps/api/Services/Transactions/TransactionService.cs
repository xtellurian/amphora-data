using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Transactions;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Transactions
{
    public class TransactionService : ITransactionService
    {
        private readonly IEntityStore<TransactionModel> store;
        private readonly IEntityStore<AmphoraModel> amphoraStore;
        private readonly IUserService userService;
        private readonly ILogger<TransactionService> logger;

        public TransactionService(
            IEntityStore<TransactionModel> store,
            IEntityStore<AmphoraModel> amphoraStore,
            IUserService userService,
            ILogger<TransactionService> logger)
        {
            this.store = store;
            this.amphoraStore = amphoraStore;
            this.userService = userService;
            this.logger = logger;
        }

        public async Task<EntityOperationResult<TransactionModel>> PurchaseAmphora(IApplicationUserReference user, AmphoraModel amphora)
        {
            var securityModel = await amphoraStore.ReadAsync<AmphoraSecurityModel>(amphora.AmphoraId, amphora.OrganisationId);
            if (securityModel.HasPurchased?.Any(u => string.Equals(u.Id, user.Id)) ?? false)
            {
                logger.LogWarning($"{user.UserName} has already purchased {amphora.Id}");
                var txs = await store.QueryAsync<TransactionModel>(p => p.UserId == user.Id && p.AmphoraId == amphora.AmphoraId);
                return new EntityOperationResult<TransactionModel>(txs.FirstOrDefault());
            }
            else
            {
                var transaction = new TransactionModel(user, amphora);
                transaction = await store.CreateAsync(transaction);
                securityModel.AddUserHasPurchased(user);
                await amphoraStore.UpdateAsync(securityModel);
                return new EntityOperationResult<TransactionModel>(transaction);
            }
        }
        public async Task<EntityOperationResult<TransactionModel>> PurchaseAmphora(ClaimsPrincipal principal, AmphoraModel amphora)
        {
            var user = await userService.UserManager.GetUserAsync(principal);
            return await this.PurchaseAmphora(user, amphora);
        }
    }
}