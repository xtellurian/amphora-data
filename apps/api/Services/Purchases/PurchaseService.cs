using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Purchases;
using Amphora.Common.Models.Users;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Transactions
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IEntityStore<PurchaseModel> purchaseStore;
        private readonly IUserService userService;
        private readonly ILogger<PurchaseService> logger;

        public PurchaseService(
            IEntityStore<PurchaseModel> purchaseStore,
            IUserService userService,
            ILogger<PurchaseService> logger)
        {
            this.purchaseStore = purchaseStore;
            this.userService = userService;
            this.logger = logger;
        }

        public async Task<EntityOperationResult<PurchaseModel>> PurchaseAmphora(ApplicationUser user, AmphoraModel amphora)
        {
            var transactions = await purchaseStore.QueryAsync(p => p.PurchasedByUserId == user.Id && p.AmphoraId == amphora.Id);
            if (transactions.Any())
            {
                logger.LogWarning($"{user.UserName} has already purchased {amphora.Id}");
                return new EntityOperationResult<PurchaseModel>(transactions.FirstOrDefault());
            }
            else
            {
                var transaction = new PurchaseModel(user, amphora);
                transaction = await purchaseStore.CreateAsync(transaction);
                return new EntityOperationResult<PurchaseModel>(transaction);
            }
        }
        public async Task<EntityOperationResult<PurchaseModel>> PurchaseAmphora(ClaimsPrincipal principal, AmphoraModel amphora)
        {
            var user = await userService.UserManager.GetUserAsync(principal);
            return await this.PurchaseAmphora(user, amphora);
        }
    }
}