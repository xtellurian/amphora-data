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
        private readonly IOrganisationService organisationService;
        private readonly FeatureFlagService featureFlags;

        public DetailModel(
            IAmphoraeService amphoraeService,
            IBlobStore<AmphoraModel> blobStore,
            IUserService userService,
            IPermissionService permissionService,
            IPurchaseService purchaseService,
            IQualityEstimatorService qualityEstimator,
            IOrganisationService organisationService,
            FeatureFlagService featureFlags) : base(amphoraeService)
        {
            this.blobStore = blobStore;
            this.userService = userService;
            this.permissionService = permissionService;
            this.purchaseService = purchaseService;
            this.qualityEstimator = qualityEstimator;
            this.organisationService = organisationService;
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
            await LoadAmphoraAsync(id);
            await SetPagePropertiesAsync();

            return OnReturnPage();
        }

        public async Task<IActionResult> OnPostPinAsync(string id, string target)
        {
            await LoadAmphoraAsync(id);
            await SetPagePropertiesAsync();

            switch (target?.ToLower())
            {
                case "pintouser":
                    return await PinToUser();
                case "pintoorg":
                    return await PinToOrg();
                case "unpinfromuser":
                    return await UnpinFromUser();
                case "unpinfromorg":
                    return await UnpinFromOrg();
            }

            return RedirectToPage("./Detail", new { Id = id });
        }

        private async Task<IActionResult> UnpinFromOrg()
        {
            var user = Result.User;
            var org = user.Organisation;
            if (!user.IsAdmin())
            {
                ModelState.AddModelError(string.Empty, "User must be an Organisation Admin");
                return OnReturnPage();
            }

            if (org.PinnedAmphorae.IsPinned(Amphora))
            {
                org.PinnedAmphorae.Unpin(Amphora);
                var res = await organisationService.UpdateAsync(User, org);
                if (!res.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, res.Message);
                }
                else
                {
                    return RedirectToPage("./Detail", new { Id = Amphora.Id });
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Amphora is not currently pinned");
            }

            return OnReturnPage();
        }

        private async Task<IActionResult> UnpinFromUser()
        {
            var user = Result.User;
            if (user.PinnedAmphorae.IsPinned(Amphora))
            {
                user.PinnedAmphorae.Unpin(Amphora);
                await userService.UserManager.UpdateAsync(user);
                return RedirectToPage("./Detail", new { Id = Amphora.Id });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Amphora is not currently pinned");
                return OnReturnPage();
            }
        }

        private async Task<IActionResult> PinToUser()
        {
            var user = Result.User;
            if (user.PinnedAmphorae.AreAnyNull())
            {
                user.PinnedAmphorae.PinToLeastNull(Amphora);
                await userService.UserManager.UpdateAsync(user);
                return RedirectToPage("./Detail", new { Id = Amphora.Id });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "No free places to pin");
                return OnReturnPage();
            }
        }

        private async Task<IActionResult> PinToOrg()
        {
            var user = Result.User;
            var org = user.Organisation;
            if (!user.IsAdmin())
            {
                ModelState.AddModelError(string.Empty, "User must be an Organisation Admin");
                return OnReturnPage();
            }

            if (org.PinnedAmphorae.AreAnyNull())
            {
                org.PinnedAmphorae.PinToLeastNull(Amphora);
                var res = await organisationService.UpdateAsync(User, org);
                if (!res.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, res.Message);
                }
                else
                {
                    return RedirectToPage("./Detail", new { Id = Amphora.Id });
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "No free places to pin");
            }

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

        public async Task<IActionResult> OnPostUploadAsync(string id, List<IFormFile> files)
        {
            if (files == null || files.Count > 1)
            {
                throw new System.ArgumentException("Only 1 file is supported");
            }

            if (string.IsNullOrEmpty(id)) { return RedirectToPage("./Index"); }

            var result = await amphoraeService.ReadAsync(User, id);
            var user = result.User;

            if (result.Succeeded)
            {
                if (result.Entity == null) { return RedirectToPage("./Index"); }

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
                return RedirectToPage("./Detail", new { Id = id });
            }
            else if (result.WasForbidden)
            {
                return RedirectToPage("./Forbidden");
            }
            else
            {
                return RedirectToPage("./Index");
            }
        }
    }
}
