using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Admin;
using Amphora.Common.Contracts;
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
        private readonly ICommissionTrackingService commissionTracking;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly ILogger<AccountsService> logger;

        public AccountsService(
            IEntityStore<PurchaseModel> purchaseStore,
            IEntityStore<OrganisationModel> orgStore,
            ICommissionTrackingService commissionTracking,
            IDateTimeProvider dateTimeProvider,
            ILogger<AccountsService> logger)
        {
            this.purchaseStore = purchaseStore;
            this.orgStore = orgStore;
            this.commissionTracking = commissionTracking;
            this.dateTimeProvider = dateTimeProvider;
            this.logger = logger;
        }

        /// <summary>
        /// Takes all the purchases from this month and creates debits in the system.
        /// </summary>
        /// <returns> An awaitable Task. </returns>
        public async Task<Report> PopulateDebitsAndCreditsAsync(DateTimeOffset? month = null)
        {
            var report = new Report();
            var startOfMonth = (month ?? dateTimeProvider.UtcNow).StartOfMonth();
            var thisMonth = purchaseStore.Query(p => p.LastDebitTime < startOfMonth || p.LastDebitTime == null);

            foreach (var purchase in thisMonth)
            {
                purchase.LastDebitTime = dateTimeProvider.UtcNow;

                try
                {
                    // check if the Amphora still exists
                    if (purchase.Amphora != null && purchase.Amphora.IsDeleted != true)
                    {
                        var name = $"Subscription Fee: {purchase.Amphora.Name} ({startOfMonth.ToString("MM/yy")})";
                        await DebitPurchasingOrganisation(purchase, name);
                        await CreditAmphoraOrganisation(purchase, name);
                        await purchaseStore.UpdateAsync(purchase);
                        report.Log($"Updated purchase {purchase.Id}");
                    }
                    else
                    {
                        // amphora has been deleted
                        await purchaseStore.DeleteAsync(purchase);
                        report.Warning($"Amphora {purchase.AmphoraId} has been deleted. Deleted purchase {purchase.Id}.");
                    }
                }
                catch (System.Exception ex)
                {
                    logger.LogError($"Error updating purchaseId: {purchase.Id}, {ex.Message}");
                    report.Log($"Error processing purchase {purchase.Id}");
                }
            }

            return report;
        }

        private async Task DebitPurchasingOrganisation(PurchaseModel purchase, string name)
        {
            // lazy loading isn't working??? Load this way instead.
            var org = await orgStore.ReadAsync(purchase.PurchasedByOrganisationId);
            if (org == null)
            {
                await purchaseStore.DeleteAsync(purchase);
                logger.LogWarning($"Purchase for non-existing org {purchase.PurchasedByOrganisationId}");
                return;
            }

            if (org.Account == null)
            {
                org.Account = new Account();
                logger.LogWarning($"New account for {org.Id}");
            }

            org.Account.DebitAccount(name, purchase.Price.Value, dateTimeProvider.UtcNow, purchase.AmphoraId);
            await orgStore.UpdateAsync(org);
        }

        private async Task CreditAmphoraOrganisation(PurchaseModel purchase, string name)
        {
            var org = await orgStore.ReadAsync(purchase.Amphora.OrganisationId);
            var commissionAmount = org.Account.CreditAccountFromSale(purchase, dateTimeProvider.UtcNow);
            await commissionTracking.TrackCommissionAsync(purchase, commissionAmount);
            org = await orgStore.UpdateAsync(org);
        }

        /// <summary>
        /// Generates and stores invoices for the provided month.
        /// </summary>
        /// <param name="month"> The month to generate invoice for.</param>
        /// <param name="organisationId"> The Organisastion Id to generate the invoice for.</param>
        /// <param name="isPreview"> Flag whether this is a preview invoice.</param>
        /// <param name="regenerate"> Override existing invoices.</param>
        /// <returns> An Invoice. </returns>
        public async Task<InvoiceModel> GenerateInvoiceAsync(System.DateTimeOffset month,
                                                        string organisationId,
                                                        bool isPreview = true,
                                                        bool regenerate = false)
        {
            if (organisationId is null)
            {
                throw new System.ArgumentNullException(nameof(organisationId));
            }

            var org = await orgStore.ReadAsync(organisationId);
            InvoiceModel invoice;
            List<InvoiceModel> toRemove = new List<InvoiceModel>();
            var existing = org.Account.Invoices().Where(_ => _.Timestamp.HasValue && _.Timestamp.Value.Month == month.Month);
            if (existing != null && existing.Any() && !regenerate)
            {
                return existing.FirstOrDefault();
            }
            else if (existing != null && existing.Any() && regenerate)
            {
                // save these to be deleted.
                toRemove.AddRange(existing);
            }

            invoice = new InvoiceModel()
            {
                Timestamp = month,
                DateCreated = dateTimeProvider.UtcNow,
                Name = $"{month.ToString("MMM", CultureInfo.InvariantCulture)} Invoice",
            };
            var som = month.StartOfMonth();
            var eom = month.EndOfMonth();
            var thisMonthsDebits = org.Account.Debits
                .Where(_ => _.Timestamp > som && _.Timestamp < eom)
                .ToList();
            var thisMonthsCredits = org.Account.Credits
                .Where(_ => _.Timestamp > som && _.Timestamp < eom)
                .ToList(); // these may be removed, so copy them to a new list
            foreach (var c in thisMonthsCredits)
            {
                invoice.Transactions.Add(new InvoiceTransaction(c));
            }

            foreach (var d in thisMonthsDebits)
            {
                invoice.Transactions.Add(new InvoiceTransaction(d));
            }

            CalculateAmounts(invoice, thisMonthsCredits, thisMonthsDebits, org.Account.Balance);
            invoice.IsPreview = isPreview;

            org.Account.Invoices().Add(invoice);

            org = await orgStore.UpdateAsync(org);

            // now remove the old ones
            foreach (var i in toRemove)
            {
                logger.LogWarning($"Removing Invoice({i.Id}) from Org({org.Id})");
                org.Account.Invoices().Remove(i);
            }

            org = await orgStore.UpdateAsync(org);

            if (!isPreview)
            {
                logger.LogInformation("Removing old transactions....");
                // include a transaction for this invoice, to account for the balance changing
                // now remove the transactions from the account
                foreach (var d in thisMonthsDebits)
                {
                    org.Account.Debits.Remove(d);
                }

                foreach (var c in thisMonthsCredits)
                {
                    org.Account.Credits.Remove(c);
                }

                // debiting account must be a positive number, when either debiting or crediting.
                var absInvoiceBalance = Math.Abs(invoice.InvoiceBalance);
                if (invoice.InvoiceBalance > 0)
                {
                    org.Account.CreditAccount($"Carry from invoice ${invoice.Id}", absInvoiceBalance, dateTimeProvider.UtcNow);
                }
                else if (invoice.InvoiceBalance < 0)
                {
                    org.Account.DebitAccount($"Carry from invoice ${invoice.Id}", absInvoiceBalance, dateTimeProvider.UtcNow);
                }

                await orgStore.UpdateAsync(org);
                logger.LogInformation("Removed old transactions.");
            }
            else
            {
                logger.LogWarning("Refusing to remove old transaction due to isPreview.");
            }

            return invoice;
        }

        public async Task<InvoiceModel> PublishInvoice(InvoiceModel invoice)
        {
            invoice.IsPreview = false;
            await orgStore.UpdateAsync(invoice.Organisation);

            // TODO: send the invoice
            return invoice;
        }

        private void CalculateAmounts(InvoiceModel invoice, IEnumerable<AccountCredit> credits, IEnumerable<AccountDebit> debits, double? openingBalance)
        {
            if (credits is null)
            {
                throw new System.ArgumentNullException(nameof(credits));
            }

            if (debits is null)
            {
                throw new System.ArgumentNullException(nameof(debits));
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
