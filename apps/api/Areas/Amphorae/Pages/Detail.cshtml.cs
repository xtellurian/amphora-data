using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Api.Services.FeatureFlags;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Permissions;
using Amphora.Common.Models.Purchases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages
{
    [Authorize]
    public class DetailModel : AmphoraPageModel
    {
        private readonly IBlobStore<AmphoraModel> blobStore;
        private readonly IUserService userService;
        private readonly IPermissionService permissionService;
        private readonly IPurchaseService purchaseService;
        private readonly IQualityEstimatorService qualityEstimator;
        private readonly FeatureFlagService featureFlags;

        public DetailModel(
            IAmphoraeService amphoraeService,
            IBlobStore<AmphoraModel> blobStore,
            IUserService userService,
            IPermissionService permissionService,
            IPurchaseService purchaseService,
            IQualityEstimatorService qualityEstimator,
            FeatureFlagService featureFlags) : base(amphoraeService)
        {
            this.blobStore = blobStore;
            this.userService = userService;
            this.permissionService = permissionService;
            this.purchaseService = purchaseService;
            this.qualityEstimator = qualityEstimator;
            this.featureFlags = featureFlags;
        }

        public DataQualitySummary Quality { get; private set; }
        public IEnumerable<string> Names { get; set; }
        
        public bool CanEditPermissions { get; set; }
        public bool CanEditDetails { get; private set; }
        public bool CanUploadFiles { get; private set; }
        public ICollection<Common.Models.Purchases.PurchaseModel> Purchases { get; private set; }
        public bool CanBuy { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await base.LoadAmphoraAsync(id);
            await SetPagePropertiesAsync();

            return OnReturnPage();
        }

        private async Task SetPagePropertiesAsync()
        {
            var user = await userService.UserManager.GetUserAsync(User);
            if (Amphora != null)
            {
                this.Quality = await qualityEstimator.GenerateDataQualitySummaryAsync(Amphora);
                Names = await blobStore.ListBlobsAsync(Amphora);
                CanEditPermissions = await permissionService.IsAuthorizedAsync(user, this.Amphora, ResourcePermissions.Create);
                // can edit permissions implies can edit details - else, check
                CanEditDetails = CanEditPermissions ? true : await permissionService.IsAuthorizedAsync(user, this.Amphora, ResourcePermissions.Update);
                CanUploadFiles = CanEditDetails ? true : await permissionService.IsAuthorizedAsync(user, this.Amphora, ResourcePermissions.WriteContents);

               
                this.Purchases = Amphora.Purchases;
                this.CanBuy = await purchaseService.CanPurchaseAmphoraAsync(User, Amphora);
            }
        }

        public async Task<IActionResult> OnPostAsync(string id, List<IFormFile> files)
        {
            if (files == null || files.Count > 1)
            {
                throw new System.ArgumentException("Only 1 file is supported");
            }

            if (string.IsNullOrEmpty(id)) return RedirectToAction("./Index");

            var result = await amphoraeService.ReadAsync(User, id);
            var user = await userService.UserManager.GetUserAsync(User);

            if (result.Succeeded)
            {
                if (result.Entity == null) return RedirectToPage("./Index");

                if (await permissionService.IsAuthorizedAsync(user, result.Entity, AccessLevels.WriteContents))
                {
                    var formFile = files.FirstOrDefault();

                    if (formFile != null && formFile.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await formFile.CopyToAsync(stream);
                            stream.Seek(0, SeekOrigin.Begin);
                            await this.blobStore.WriteBytesAsync(result.Entity, formFile.FileName, await stream.ReadFullyAsync());
                        }
                    }
                }
                else
                {
                    return RedirectToPage("./Forbidden");
                }
                this.Amphora = result.Entity;
                await SetPagePropertiesAsync();
                return Page();
            }
            else if (result.WasForbidden)
            {
                return RedirectToPage("./Forbidden");
            }
            else
            {
                return RedirectToPage(".Index");
            }

        }

        
    }
}