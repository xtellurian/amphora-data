using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Organisations.Accounts;
using Amphora.Common.Models.Purchases;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Purchases
{
    public class AccountsService : IAccountsService
    {
        private readonly IEntityStore<PurchaseModel> purchaseStore;
        private readonly IEntityStore<OrganisationModel> orgStore;
        private readonly ILogger<AccountsService> logger;

        public AccountsService(
            IEntityStore<PurchaseModel> purchaseStore,
            IEntityStore<OrganisationModel> orgStore,
            ILogger<AccountsService> logger)
        {
            this.purchaseStore = purchaseStore;
            this.orgStore = orgStore;
            this.logger = logger;
        }
        // does it for this current month
        public async Task PopulateDebitsAsync()
        {
            var now = System.DateTime.UtcNow;
            var startOfMonth = new System.DateTime(now.Year, now.Month, 1, 0, 0, 0, 0);
            var thisMonth = await purchaseStore.QueryAsync(p => p.LastDebitTime < startOfMonth || p.LastDebitTime == null);

            foreach (var purchase in thisMonth)
            {
                if (purchase.Price.HasValue)
                {
                    // lazy loading isn't working??? Load this way instead.
                    var org = await orgStore.ReadAsync(purchase.PurchasedByOrganisationId);
                    if(org == null) 
                    {
                        await purchaseStore.DeleteAsync(purchase);
                        logger.LogWarning($"Purchase for non-existing org {purchase.PurchasedByOrganisationId}");
                        continue;
                    }

                    if (org.Account == null) 
                    {
                        org.Account = new Account();
                        logger.LogWarning($"New account for {org.Id}");
                    }

                    org.Account.DebitAccount($"{purchase.AmphoraId} ({startOfMonth.ToString("MMMM")})", purchase.Price.Value);
                    await orgStore.UpdateAsync(org);
                }
                purchase.LastDebitTime = System.DateTime.UtcNow;
                await purchaseStore.UpdateAsync(purchase);
            }



            // foreach(var o in updatedOrgs)
            // {
            //     await orgStore.UpdateAsync(o);
            // }

        }
    }
}