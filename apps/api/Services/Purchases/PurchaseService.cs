using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Purchases;
using Amphora.Common.Models.Users;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Services.Transactions
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IEntityStore<PurchaseModel> purchaseStore;
        private readonly IUserService userService;
        private readonly IEmailSender emailSender;
        private readonly ILogger<PurchaseService> logger;

        public PurchaseService(
            IEntityStore<PurchaseModel> purchaseStore,
            IUserService userService,
            IEmailSender emailSender,
            ILogger<PurchaseService> logger)
        {
            this.purchaseStore = purchaseStore;
            this.userService = userService;
            this.emailSender = emailSender;
            this.logger = logger;
        }

        public async Task<EntityOperationResult<PurchaseModel>> PurchaseAmphora(ApplicationUser user, AmphoraModel amphora)
        {
            using (logger.BeginScope(new LoggerScope<PurchaseService>(user)))
            {
                var purchases = await purchaseStore.QueryAsync(p => p.PurchasedByUserId == user.Id && p.AmphoraId == amphora.Id);
                if (purchases.Any())
                {
                    logger.LogWarning($"{user.UserName} has already purchased {amphora.Id}");
                    return new EntityOperationResult<PurchaseModel>(purchases.FirstOrDefault());
                }
                else
                {
                    logger.LogTrace("Purchasing Amphora");
                    var purchase = new PurchaseModel(user, amphora);
                    purchase = await purchaseStore.CreateAsync(purchase);
                    await SendPurchaseConfimationEmail(purchase);
                    return new EntityOperationResult<PurchaseModel>(purchase);
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
            if(user?.Organisation?.TermsAndConditionsAccepted == null) return false;
            if(amphora.TermsAndConditionsId == null) return true; // no terms and conditions

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

        public async Task<EntityOperationResult<PurchaseModel>> PurchaseAmphora(ClaimsPrincipal principal, AmphoraModel amphora)
        {
            var user = await userService.UserManager.GetUserAsync(principal);
            return await this.PurchaseAmphora(user, amphora);
        }
    }
}