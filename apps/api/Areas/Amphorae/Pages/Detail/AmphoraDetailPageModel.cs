using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Permissions;
using Amphora.Common.Models.Purchases;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages.Detail
{
    public abstract class AmphoraDetailPageModel : AmphoraPageModel
    {
        protected readonly IQualityEstimatorService qualityEstimator;
        protected readonly IPurchaseService purchaseService;
        protected readonly IPermissionService permissionService;

        public AmphoraDetailPageModel(IAmphoraeService amphoraeService,
                                      IQualityEstimatorService qualityEstimator,
                                      IPurchaseService purchaseService,
                                      IPermissionService permissionService) : base(amphoraeService)
        {
            this.qualityEstimator = qualityEstimator;
            this.purchaseService = purchaseService;
            this.permissionService = permissionService;
        }

        public int FileCount => this.Quality.CountFiles ?? 0;
        public int SignalCount => this.Quality.CountSignals ?? 0;

        public DataQualitySummary Quality { get; private set; }
        public bool CanAdminister { get; private set; }
        public bool CanUpdate { get; private set; }
        public bool CanWriteContents { get; private set; }
        public bool CanReadContents { get; private set; }
        public bool CanDeleteFiles { get; private set; }
        public ICollection<PurchaseModel> Purchases { get; private set; }
        public bool CanBuy { get; private set; }
        public PurchaseModel Purchase { get; private set; }
        public bool HasPurchased => Purchase != null;

        public virtual async Task<IActionResult> OnPostPurchaseAsync(string id)
        {
            await LoadAmphoraAsync(id);
            await SetPagePropertiesAsync();

            if (Amphora != null)
            {
                var hasAgreed = await purchaseService.HasAgreedToTermsAndConditionsAsync(User, Amphora);
                var canPurchase = await purchaseService.CanPurchaseAmphoraAsync(User, Amphora);
                if (!canPurchase)
                {
                    ModelState.AddModelError(string.Empty, "Cannot purchase");
                    return Page();
                }

                if (hasAgreed)
                {
                    await purchaseService.PurchaseAmphoraAsync(User, Amphora);
                    await SetPagePropertiesAsync();
                    return Page();
                }
                else
                {
                    return RedirectToPage("/Detail/TermsOfUse", new { id = id, promptAccept = true });
                }
            }
            else
            {
                return RedirectToPage("/Index");
            }
        }

        protected virtual async Task SetPagePropertiesAsync()
        {
            if (Amphora != null)
            {
                this.Quality = await qualityEstimator.GenerateDataQualitySummaryAsync(Amphora);
                CanAdminister = await permissionService.IsAuthorizedAsync(User, Amphora, AccessLevels.Administer);
                CanUpdate = CanAdminister ? true : await permissionService.IsAuthorizedAsync(User, this.Amphora, AccessLevels.Update);
                CanWriteContents = CanUpdate ? true : await permissionService.IsAuthorizedAsync(User, this.Amphora, ResourcePermissions.WriteContents);
                CanReadContents = CanWriteContents ? true : await permissionService.IsAuthorizedAsync(User, this.Amphora, ResourcePermissions.ReadContents);
                CanDeleteFiles = CanWriteContents;

                this.Purchases = Amphora.Purchases;
                this.CanBuy = await purchaseService.CanPurchaseAmphoraAsync(User, Amphora);
                this.Purchase = Amphora.Purchases.FirstOrDefault(_ => _.PurchasedByOrganisationId == Result.User.OrganisationId);
            }
        }
    }
}