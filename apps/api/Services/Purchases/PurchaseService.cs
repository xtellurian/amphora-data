using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Events;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Organisations.Accounts;
using Amphora.Common.Models.Purchases;
using Amphora.Common.Models.Users;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Purchases
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IEntityStore<PurchaseModel> purchaseStore;
        private readonly IEntityStore<OrganisationModel> orgStore;
        private readonly IPermissionService permissionService;
        private readonly IUserService userService;
        private readonly IEmailSender emailSender;
        private readonly IEventPublisher eventPublisher;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly ILogger<PurchaseService> logger;

        public PurchaseService(
            IEntityStore<PurchaseModel> purchaseStore,
            IEntityStore<OrganisationModel> orgStore,
            IPermissionService permissionService,
            IUserService userService,
            IEmailSender emailSender,
            IEventPublisher eventPublisher,
            IDateTimeProvider dateTimeProvider,
            ILogger<PurchaseService> logger)
        {
            this.purchaseStore = purchaseStore;
            this.orgStore = orgStore;
            this.permissionService = permissionService;
            this.userService = userService;
            this.emailSender = emailSender;
            this.eventPublisher = eventPublisher;
            this.dateTimeProvider = dateTimeProvider;
            this.logger = logger;
        }

        public async Task<bool> CanPurchaseAmphoraAsync(ApplicationUser user, AmphoraModel amphora)
        {
            if (user.OrganisationId == amphora.OrganisationId) { return false; }
            var alreadyPurchased = amphora.Purchases?.Any(u => string.Equals(u.PurchasedByOrganisationId, user.OrganisationId)) ?? false;
            return !alreadyPurchased && await permissionService.IsAuthorizedAsync(user, amphora, Common.Models.Permissions.AccessLevels.Purchase);
        }

        public async Task<bool> CanPurchaseAmphoraAsync(ClaimsPrincipal principal, AmphoraModel amphora)
        {
            var user = await userService.ReadUserModelAsync(principal);
            return await this.CanPurchaseAmphoraAsync(user, amphora);
        }

        public async Task<EntityOperationResult<PurchaseModel>> PurchaseAmphoraAsync(ApplicationUser user, AmphoraModel amphora)
        {
            if (user.OrganisationId == null) { return new EntityOperationResult<PurchaseModel>(user, "User has no organisation"); }
            using (logger.BeginScope(new LoggerScope<PurchaseService>(user)))
            {
                if (!await CanPurchaseAmphoraAsync(user, amphora)) { return new EntityOperationResult<PurchaseModel>(user, "Purchase permission denied"); }
                var purchases = await purchaseStore.QueryAsync(p => p.PurchasedByUserId == user.Id && p.AmphoraId == amphora.Id);
                if (purchases.Any())
                {
                    logger.LogWarning($"{user.UserName} has already purchased {amphora.Id}");
                    return new EntityOperationResult<PurchaseModel>(user, purchases.FirstOrDefault());
                }
                else
                {
                    logger.LogTrace("Purchasing Amphora");
                    var purchase = new PurchaseModel(user, amphora);
                    purchase = await purchaseStore.CreateAsync(purchase);
                    await SendPurchaseConfimationEmail(purchase);
                    if (purchase.Price.HasValue)
                    {
                        // debit the account initially
                        logger.LogInformation($"Debiting account {purchase.Price.Value}");
                        var org = await orgStore.ReadAsync(purchase.PurchasedByOrganisationId);
                        if (org.Account == null) { org.Account = new Account(); }
                        org.Account.DebitAccountFromPurchase(purchase, dateTimeProvider.UtcNow);
                        org = await orgStore.UpdateAsync(org);

                        // credit the selling account
                        var sellerOrg = await orgStore.ReadAsync(purchase.Amphora.OrganisationId);
                        if (sellerOrg.Account == null) { sellerOrg.Account = new Account(); }
                        sellerOrg.Account.CreditAccountFromSale(purchase, dateTimeProvider.UtcNow);
                        sellerOrg = await orgStore.UpdateAsync(sellerOrg);
                    }
                    else
                    {
                        logger.LogWarning($"Amphora {amphora.Id} has no Price.");
                    }

                    // update the purchase and the amphora
                    amphora.PurchaseCount ??= 0;
                    amphora.PurchaseCount = await purchaseStore.CountAsync(_ => _.AmphoraId == amphora.Id);
                    purchase.LastDebitTime = DateTime.UtcNow;
                    purchase = await purchaseStore.UpdateAsync(purchase);
                    await eventPublisher.PublishEventAsync(new PurchaseEvent(purchase));

                    return new EntityOperationResult<PurchaseModel>(user, purchase);
                }
            }
        }

        public async Task<bool> HasAgreedToTermsAndConditionsAsync(ClaimsPrincipal principal, AmphoraModel amphora)
        {
            var user = await userService.ReadUserModelAsync(principal);
            return this.HasAgreedToTermsAndConditions(user, amphora);
        }

        public bool HasAgreedToTermsAndConditions(ApplicationUser user, AmphoraModel amphora)
        {
            if (amphora.TermsAndConditionsId == null) { return true; } // no terms and conditions
            if (user.OrganisationId == amphora.OrganisationId) { return true; } // no need to accept your own terms and conditions
            if (user?.Organisation?.TermsAndConditionsAccepted == null) { return false; }

            return user.Organisation.TermsAndConditionsAccepted.Any(t =>
                t.TermsAndConditionsOrganisationId == amphora.OrganisationId
                && t.TermsAndConditionsId == amphora.TermsAndConditionsId);
        }

        private async Task SendPurchaseConfimationEmail(PurchaseModel purchase)
        {
            if (purchase.PurchasedByUser.EmailConfirmed)
            {
                await emailSender.SendEmailAsync(purchase.PurchasedByUser.Email,
                    "You've purchased a new Amphora",
                    $"Your new Amphora ({purchase.Amphora.Name}) is now available at Amphora Data");
            }
            else
            {
                logger.LogWarning($"Cannot send email to {purchase.PurchasedByUser.Email}. Email unconfirmed");
            }
        }

        public async Task<EntityOperationResult<PurchaseModel>> PurchaseAmphoraAsync(ClaimsPrincipal principal, AmphoraModel amphora)
        {
            var user = await userService.UserManager.GetUserAsync(principal);
            return await this.PurchaseAmphoraAsync(user, amphora);
        }
    }
}