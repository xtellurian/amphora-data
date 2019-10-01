using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Transactions;
using Amphora.Common.Models.Users;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Transactions
{
    public class TransactionService : ITransactionService
    {
        private readonly IEntityStore<TransactionModel> transactionStore;
        private readonly IUserService userService;
        private readonly ILogger<TransactionService> logger;

        public TransactionService(
            IEntityStore<TransactionModel> transactionStore,
            IUserService userService,
            ILogger<TransactionService> logger)
        {
            this.transactionStore = transactionStore;
            this.userService = userService;
            this.logger = logger;
        }

        public async Task<EntityOperationResult<TransactionModel>> PurchaseAmphora(ApplicationUser user, AmphoraModel amphora)
        {
            var transactions = await transactionStore.QueryAsync(p => p.UserId == user.Id && p.AmphoraId == amphora.Id);
            if (transactions.Any())
            {
                logger.LogWarning($"{user.UserName} has already purchased {amphora.Id}");
                return new EntityOperationResult<TransactionModel>(transactions.FirstOrDefault());
            }
            else
            {
                var transaction = new TransactionModel(user, amphora);
                transaction = await transactionStore.CreateAsync(transaction);
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