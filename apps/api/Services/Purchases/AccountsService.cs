using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Extensions;
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

        /// <summary>
        /// Takes all the purchases from this month and creates debits in the system.
        /// </summary>
        /// <returns> An awaitable Task. </returns>
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

                    org.Account.DebitAccount($"Subscription {purchase.AmphoraId} ({startOfMonth.ToString("MMMM")})", purchase.Price.Value, purchase.AmphoraId);
                    await orgStore.UpdateAsync(org);
                }

                purchase.LastDebitTime = System.DateTime.UtcNow;
                await purchaseStore.UpdateAsync(purchase);
            }
        }

        /// <summary>
        /// Generates and stores invoices for the provided month.
        /// </summary>
        /// <param name="month"> The month to generate invoice for.</param>
        /// <param name="organisationId"> The Organisastion Id to generate the invoice for.</param>
        /// <param name="isPreview"> Flag whether this is a preview invoice.</param>
        /// <param name="regenerate"> Override existing invoices.</param>
        /// <returns> An Invoice. </returns>
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
            var som = month.StartOfMonth();
            var eom = month.EndOfMonth();
            var thisMonthsDebits = org.Account.Debits
                .Where(_ => _.CreatedDate > som && _.CreatedDate < eom);
            var thisMonthsCredits = org.Account.Credits
                .Where(_ => _.CreatedDate > som && _.CreatedDate < eom);
            foreach (var c in thisMonthsCredits)
            {
                invoice.Transactions.Add(new InvoiceTransaction(c.Label, c.Amount, isCredit: true));
            }

            foreach (var d in thisMonthsDebits)
            {
                invoice.Transactions.Add(new InvoiceTransaction(d.Label, d.Amount, isDebit: true) { AmphoraId = d.AmphoraId });
            }

            CalculateAmounts(invoice, thisMonthsCredits, thisMonthsDebits, org.Account.Balance);
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

        private void CalculateAmounts(Invoice invoice, IEnumerable<AccountCredit> credits, IEnumerable<AccountDebit> debits, double? openingBalance)
        {
            if (credits is null)
            {
                throw new ArgumentNullException(nameof(credits));
            }

            if (debits is null)
            {
                throw new ArgumentNullException(nameof(debits));
            }

            invoice.TotalCredits ??= 0;
            invoice.TotalDebits ??= 0;

            if (invoice.CountCredits > 0)
            {
                invoice.TotalCredits += credits.Sum(c => c.Amount) ?? 0;
            }

            if (invoice.CountDebits > 0)
            {
                invoice.TotalDebits += debits.Sum(d => d.Amount) ?? 0;
            }

            invoice.OpeningBalance = openingBalance;
        }
    }
}
