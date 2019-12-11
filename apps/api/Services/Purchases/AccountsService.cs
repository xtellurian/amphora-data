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
        public async Task<Invoice> GenerateInvoiceAsync(DateTimeOffset month,
                                                        string organisationId,
                                                        bool isPreview = true,
                                                        bool regenerate = false)
        {
            if (organisationId is null)
            {
                throw new ArgumentNullException(nameof(organisationId));
            }

            var org = await orgStore.ReadAsync(organisationId);
            Invoice invoice;
            var existing = org.Account.Invoices.FirstOrDefault(_ => _.DateCreated.HasValue && _.DateCreated.Value.Month == month.Month);
            if (existing != null && !regenerate)
            {
                return null;
            }

            invoice = new Invoice()
            {
                DateCreated = DateTime.UtcNow,
                Name = $"{month.ToString("MMM", CultureInfo.InvariantCulture)} Invoice"
            };

            var thisMonthsDebits = org.Account.Debits
                .Where(_ => _.CreatedDate > month.StartOfMonth() && _.CreatedDate < month.EndOfMonth());
            var thisMonthsCredits = org.Account.Credits
                .Where(_ => _.CreatedDate > month.StartOfMonth() && _.CreatedDate < month.EndOfMonth());
            foreach (var c in thisMonthsCredits)
            {
                invoice.Credits.Add(new InvoiceCredit(c.Label, c.Amount));
            }
            foreach (var d in thisMonthsDebits)
            {
                invoice.Debits.Add(new InvoiceDebit(d.Label, d.Amount));
            }

            invoice.NumberOfCredits = thisMonthsCredits.Count();
            invoice.NumberOfDebits = thisMonthsDebits.Count();
            invoice.Balance = CalculateBalance(thisMonthsCredits, thisMonthsDebits);
            invoice.IsPreview = isPreview;

            org.Account.Invoices.Add(invoice);

            await orgStore.UpdateAsync(org);

            return invoice;
        }

        public async Task<Invoice> PublishInvoice(Invoice invoice)
        {
            invoice.IsPreview = false;
            await orgStore.UpdateAsync(invoice.Account.Organisation);

            // TODO: send the invoice
            return invoice;
        }

        private double CalculateBalance(IEnumerable<AccountCredit> credits, IEnumerable<AccountDebit> debits)
        {
            if (credits is null)
            {
                throw new ArgumentNullException(nameof(credits));
            }

            if (debits is null)
            {
                throw new ArgumentNullException(nameof(debits));
            }

            double credit = 0;
            double debit = 0;
            if (credits.Count() > 0)
            {
                credit += credits.Sum(c => c.Amount) ?? 0;
            }
            if (debits.Count() > 0)
            {
                debit += debits.Sum(d => d.Amount) ?? 0;
            }
            return credit - debit;
        }
    }
}
