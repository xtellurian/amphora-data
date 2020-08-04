using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Events;
using Amphora.Common.Models.Logging;
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
        private readonly ICommissionTrackingService commissionTracker;
        private readonly IUserDataService userDataService;
        private readonly IEmailSender emailSender;
        private readonly IEventRoot eventPublisher;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly ILogger<PurchaseService> logger;

        public PurchaseService(
            IEntityStore<PurchaseModel> purchaseStore,
            IEntityStore<OrganisationModel> orgStore,
            IPermissionService permissionService,
            ICommissionTrackingService commissionTracker,
            IUserDataService userDataService,
            IEmailSender emailSender,
            IEventRoot eventPublisher,
            IDateTimeProvider dateTimeProvider,
            ILogger<PurchaseService> logger)
        {
            this.purchaseStore = purchaseStore;
            this.orgStore = orgStore;
            this.permissionService = permissionService;
            this.commissionTracker = commissionTracker;
            this.userDataService = userDataService;
            this.emailSender = emailSender;
            this.eventPublisher = eventPublisher;
            this.dateTimeProvider = dateTimeProvider;
            this.logger = logger;
        }

        public async Task<bool> CanPurchaseAmphoraAsync(ApplicationUserDataModel user, AmphoraModel amphora)
        {
            if (user == null) { return false; }
            if (user.OrganisationId == amphora.OrganisationId) { return false; }
            if (HasAgreedToTerms(user, amphora))
            {
                var alreadyPurchased = amphora.Purchases?.Any(u => string.Equals(u.PurchasedByOrganisationId, user.OrganisationId)) ?? false;
                return !alreadyPurchased && await permissionService.IsAuthorizedAsync(user, amphora, Common.Models.Permissions.AccessLevels.Purchase);
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> CanPurchaseAmphoraAsync(ClaimsPrincipal principal, AmphoraModel amphora)
        {
            var userReadRes = await userDataService.ReadAsync(principal);
            if (userReadRes.Succeeded)
            {
                return await this.CanPurchaseAmphoraAsync(userReadRes.Entity, amphora);
            }
            else
            {
                return false;
            }
        }

        public async Task<EntityOperationResult<PurchaseModel>> PurchaseAmphoraAsync(ApplicationUserDataModel user, AmphoraModel amphora)
        {
            if (user.OrganisationId == null) { return new EntityOperationResult<PurchaseModel>(user, "User has no organisation"); }
            using (logger.BeginScope(new LoggerScope<PurchaseService>(user)))
            {
                if (!await CanPurchaseAmphoraAsync(user, amphora)) { return new EntityOperationResult<PurchaseModel>(user, "Purchase permission denied"); }
                var purchases = await purchaseStore.QueryAsync(p => p.PurchasedByUserId == user.Id && p.AmphoraId == amphora.Id, 0, 1);
                if (purchases.Any())
                {
                    logger.LogWarning($"{user.UserName} has already purchased {amphora.Id}");
                    return new EntityOperationResult<PurchaseModel>(user, purchases.FirstOrDefault());
                }
                else
                {
                    logger.LogInformation($"Purchasing Amphora({amphora.Id})");
                    var purchase = new PurchaseModel(user, amphora, dateTimeProvider.UtcNow);
                    purchase = await purchaseStore.CreateAsync(purchase);
                    await SendPurchaseConfimationEmail(purchase);
                    purchase.Price ??= 0; // set 0 if

                    // debit the account initially
                    logger.LogInformation($"Debiting account {purchase.Price.Value}");
                    var org = await orgStore.ReadAsync(purchase.PurchasedByOrganisationId);
                    org.Account ??= new Account();
                    org.Account.DebitAccountFromPurchase(purchase, dateTimeProvider.UtcNow);
                    org = await orgStore.UpdateAsync(org);

                    // credit the selling account
                    var sellerOrg = await orgStore.ReadAsync(purchase.Amphora.OrganisationId);
                    sellerOrg.Account ??= new Account();
                    var commission = sellerOrg.Account.CreditAccountFromSale(purchase, dateTimeProvider.UtcNow);
                    sellerOrg = await orgStore.UpdateAsync(sellerOrg);
                    await commissionTracker.TrackCommissionAsync(purchase, commission);

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

        public async Task<bool> HasAgreedToTermsAsync(ClaimsPrincipal principal, AmphoraModel amphora)
        {
            var userReadRes = await userDataService.ReadAsync(principal);
            if (userReadRes.Succeeded)
            {
                return this.HasAgreedToTerms(userReadRes.Entity, amphora);
            }
            else
            {
                return false;
            }
        }

        public bool HasAgreedToTerms(ApplicationUserDataModel user, AmphoraModel amphora)
        {
            if (amphora.TermsOfUseId == null) { return true; } // no terms
            if (user.OrganisationId == amphora.OrganisationId) { return true; } // no need to accept your own terms
            if (user?.Organisation?.TermsOfUsesAccepted == null) { return false; }

            return user.Organisation.TermsOfUsesAccepted.Any(t => t.TermsOfUseId == amphora.TermsOfUseId);
        }

        private async Task SendPurchaseConfimationEmail(PurchaseModel purchase)
        {
            if (purchase.PurchasedByUser?.ContactInformation?.EmailConfirmed == true)
            {
                await emailSender.SendEmailAsync(purchase.PurchasedByUser?.ContactInformation?.Email,
                    "You've purchased a new Amphora",
                    $"Your new Amphora ({purchase.Amphora.Name}) is now available at Amphora Data");
            }
            else
            {
                logger.LogInformation($"Refusing to send email to {purchase.PurchasedByUser?.ContactInformation?.Email}. Email was unconfirmed");
            }
        }

        public async Task<EntityOperationResult<PurchaseModel>> PurchaseAmphoraAsync(ClaimsPrincipal principal, AmphoraModel amphora)
        {
            var userReadRes = await userDataService.ReadAsync(principal);
            if (userReadRes.Succeeded)
            {
                return await this.PurchaseAmphoraAsync(userReadRes.Entity, amphora);
            }
            else
            {
                return new EntityOperationResult<PurchaseModel>(false);
            }
        }
    }
}