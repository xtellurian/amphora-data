using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Organisations.Accounts;
using Amphora.Common.Models.Purchases;
using Amphora.Common.Extensions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

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
        /// <summary>
        /// Takes all the purchases from this month and creates debits in the system.
        /// </summary>
        public async Task PopulateDebitsAsync()
        {
            var startOfMonth = System.DateTime.UtcNow.StartOfMonth();
            var thisMonth = await purchaseStore.QueryAsync(p => p.LastDebitTime < startOfMonth || p.LastDebitTime == null);

            foreach (var purchase in thisMonth)
            {
                if (purchase.Price.HasValue)
                {
                    // lazy loading isn't working??? Load this way instead.
                    var org = await orgStore.ReadAsync(purchase.PurchasedByOrganisationId);
                    if (org == null)
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
        }
        /// <summary>
        /// Generates and stores invoices for the provided month
        /// </summary>
        public async Task<IEnumerable<Invoice>> GenerateInvoicesAsync(DateTimeOffset month, bool regenerate = false)
        {
            var allOrgs = orgStore.Query(_ => true);
            var invoices = new List<Invoice>();
            foreach (var org in allOrgs)
            {
                Invoice invoice;
                var existing = org.Account.Invoices.FirstOrDefault(_ => _.DateCreated.HasValue && _.DateCreated.Value.Month == month.Month);
                if (existing != null && regenerate)
                {
                    existing.Credits = new List<InvoiceCredit>();
                    existing.Debits = new List<InvoiceDebit>();
                    invoice = existing;
                }
                else if (existing == null)
                {
                    invoice = new Invoice()
                    {
                        DateCreated = DateTime.UtcNow,
                        Name = $"{DateTime.UtcNow.ToString("MMM", CultureInfo.InvariantCulture)} Invoice"
                    };
                }
                else
                {
                    // invoice exists, and don't regenerate
                    continue;
                }

                var thisMonthsDebits = org.Account.Debits
                    .Where(_ => _.CreatedDate > month.StartOfMonth() && _.CreatedDate < month.EndOfMonth())
                    .Select(_ => new InvoiceDebit(_.Label, _.Amount)
                    {
                        CreatedDate = _.CreatedDate,
                    });
                var thisMonthsCredits = org.Account.Credits
                    .Where(_ => _.CreatedDate > month.StartOfMonth() && _.CreatedDate < month.EndOfMonth())
                    .Select(_ => new InvoiceCredit(_.Label, _.Amount)
                    {
                        CreatedDate = _.CreatedDate,
                    });

                invoice.Credits.AddRange(thisMonthsCredits);
                invoice.Debits.AddRange(thisMonthsDebits);
                org.Account.Invoices.Add(invoice);

                await orgStore.UpdateAsync(org);
                invoices.Add(invoice);
            }

            return invoices;
        }
    }
}
