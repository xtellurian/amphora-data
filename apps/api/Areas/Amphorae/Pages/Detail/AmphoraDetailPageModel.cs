using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Purchases;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages.Detail
{
    public class AmphoraDetailPageModel : AmphoraPageModel
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
        public bool CanEditPermissions { get; private set; }
        public bool CanEditDetails { get; private set; }
        public bool CanUploadFiles { get; private set; }
        public ICollection<PurchaseModel> Purchases { get; private set; }
        public bool CanBuy { get; private set; }
        public PurchaseModel Purchase { get; private set; }

        protected async Task SetPagePropertiesAsync()
        {
            if (Amphora != null)
            {
                this.Quality = await qualityEstimator.GenerateDataQualitySummaryAsync(Amphora);
                // Names = await blobStore.ListBlobsAsync(Amphora);
                CanEditPermissions = await permissionService.IsAuthorizedAsync(User, this.Amphora, ResourcePermissions.Create);
                // can edit permissions implies can edit details - else, check
                CanEditDetails = CanEditPermissions ? true : await permissionService.IsAuthorizedAsync(User, this.Amphora, ResourcePermissions.Update);
                CanUploadFiles = CanEditDetails ? true : await permissionService.IsAuthorizedAsync(User, this.Amphora, ResourcePermissions.WriteContents);

                this.Purchases = Amphora.Purchases;
                this.CanBuy = await purchaseService.CanPurchaseAmphoraAsync(User, Amphora);
            }
        }

        protected void TryLoadPurchase()
        {
            if (this.Amphora != null)
            {
                this.Purchase = Amphora.Purchases.FirstOrDefault(_ => _.PurchasedByOrganisationId == Result.User.OrganisationId);
            }
        }
    }
}