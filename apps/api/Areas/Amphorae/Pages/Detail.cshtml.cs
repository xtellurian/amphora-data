using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Services.FeatureFlags;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.GitHub;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages
{
    [CommonAuthorize]
    public class DetailModel : AmphoraPageModel
    {
        private readonly IBlobStore<AmphoraModel> blobStore;
        private readonly IUserDataService userDataService;
        private readonly IPermissionService permissionService;
        private readonly IPurchaseService purchaseService;
        private readonly IQualityEstimatorService qualityEstimator;
        private readonly IOrganisationService organisationService;
        private readonly IAmphoraGitHubIssueConnectorService github;
        private readonly FeatureFlagService featureFlags;

        public DetailModel(
            IAmphoraeService amphoraeService,
            IBlobStore<AmphoraModel> blobStore,
            IUserDataService userDataService,
            IPermissionService permissionService,
            IPurchaseService purchaseService,
            IQualityEstimatorService qualityEstimator,
            IOrganisationService organisationService,
            IAmphoraGitHubIssueConnectorService github,
            FeatureFlagService featureFlags) : base(amphoraeService)
        {
            this.blobStore = blobStore;
            this.userDataService = userDataService;
            this.permissionService = permissionService;
            this.purchaseService = purchaseService;
            this.qualityEstimator = qualityEstimator;
            this.organisationService = organisationService;
            this.github = github;
            this.featureFlags = featureFlags;
        }

        public DataQualitySummary Quality { get; private set; }
        public IEnumerable<string> Names { get; set; }

        public bool CanEditPermissions { get; set; }
        public bool CanEditDetails { get; private set; }
        public bool CanUploadFiles { get; private set; }
        public int FileCount => this.Quality.CountFiles ?? 0;
        public int SignalCount => this.Quality.CountSignals ?? 0;
        public ICollection<Common.Models.Purchases.PurchaseModel> Purchases { get; private set; }
        public bool CanBuy { get; set; }
        public IReadOnlyList<LinkedGitHubIssue> Issues { get; private set; }
        public Common.Models.Purchases.PurchaseModel Purchase { get; private set; }
        public ApplicationUserDataModel UserData { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await LoadAmphoraAsync(id);
            await SetPagePropertiesAsync();
            TryLoadPurchase();
            return OnReturnPage();
        }

        private void TryLoadPurchase()
        {
            if (this.Amphora != null)
            {
                this.Purchase = Amphora.Purchases.FirstOrDefault(_ => _.PurchasedByOrganisationId == Result.User.OrganisationId);
            }
        }

        public async Task<IActionResult> OnPostPinAsync(string id, string target)
        {
            await LoadAmphoraAsync(id);
            await SetPagePropertiesAsync();
            TryLoadPurchase();

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
            var res = await organisationService.ReadAsync(User, user.OrganisationId);

            if (res.Succeeded && !res.Entity.IsAdministrator(user))
            {
                ModelState.AddModelError(string.Empty, "User must be an Organisation Admin");
                return OnReturnPage();
            }

            var org = res.Entity;
            if (org.PinnedAmphorae.IsPinned(Amphora))
            {
                org.PinnedAmphorae.Unpin(Amphora);
                var updateRes = await organisationService.UpdateAsync(User, org);
                if (!updateRes.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, updateRes.Message);
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
            var userReadRes = await userDataService.ReadAsync(User);
            if (userReadRes.Succeeded)
            {
                this.UserData = userReadRes.Entity;
                if (UserData.PinnedAmphorae.IsPinned(Amphora))
                {
                    UserData.PinnedAmphorae.Unpin(Amphora);
                    await userDataService.UpdateAsync(User, UserData);
                    return RedirectToPage("./Detail", new { Id = Amphora.Id });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Amphora is not currently pinned");
                    return OnReturnPage();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, userReadRes.Message);
                return OnReturnPage();
            }
        }

        private async Task<IActionResult> PinToUser()
        {
            var userReadRes = await userDataService.ReadAsync(User);
            if (userReadRes.Succeeded)
            {
                UserData = userReadRes.Entity;
                if (UserData.PinnedAmphorae.AreAnyNull())
                {
                    UserData.PinnedAmphorae.PinToLeastNull(Amphora);
                    await userDataService.UpdateAsync(User, UserData);
                    return RedirectToPage("./Detail", new { Id = Amphora.Id });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "No free places to pin");
                    return OnReturnPage();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, userReadRes.Message);
                return OnReturnPage();
            }
        }

        private async Task<IActionResult> PinToOrg()
        {
            var user = Result.User;
            var res = await organisationService.ReadAsync(User, user.OrganisationId);

            if (res.Succeeded && !res.Entity.IsAdministrator(user))
            {
                ModelState.AddModelError(string.Empty, "User must be an Organisation Admin");
                return OnReturnPage();
            }

            var org = res.Entity;
            if (org.PinnedAmphorae.AreAnyNull())
            {
                org.PinnedAmphorae.PinToLeastNull(Amphora);
                var updateRes = await organisationService.UpdateAsync(User, org);
                if (!updateRes.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, updateRes.Message);
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
            var userReadRes = await userDataService.ReadAsync(User);
            if (userReadRes.Succeeded && Amphora != null)
            {
                var userData = userReadRes.Entity;
                this.Quality = await qualityEstimator.GenerateDataQualitySummaryAsync(Amphora);
                Names = await blobStore.ListBlobsAsync(Amphora);
                CanEditPermissions = await permissionService.IsAuthorizedAsync(userData, this.Amphora, ResourcePermissions.Create);
                // can edit permissions implies can edit details - else, check
                CanEditDetails = CanEditPermissions ? true : await permissionService.IsAuthorizedAsync(userData, this.Amphora, ResourcePermissions.Update);
                CanUploadFiles = CanEditDetails ? true : await permissionService.IsAuthorizedAsync(userData, this.Amphora, ResourcePermissions.WriteContents);

                this.Purchases = Amphora.Purchases;
                this.CanBuy = await purchaseService.CanPurchaseAmphoraAsync(User, Amphora);
            }
        }
    }
}
