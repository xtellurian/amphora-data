using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages.Detail
{
    [CommonAuthorize]
    public class OptionsPageModel : AmphoraDetailPageModel
    {
        private readonly IOrganisationService organisationService;
        private readonly IUserDataService userDataService;

        public ApplicationUserDataModel UserData { get; private set; }

        public OptionsPageModel(IAmphoraeService amphoraeService,
                                   IQualityEstimatorService qualityEstimator,
                                   IPurchaseService purchaseService,
                                   IPermissionService permissionService,
                                   IOrganisationService organisationService,
                                   IUserDataService userDataService) : base(amphoraeService,
                                                                                qualityEstimator,
                                                                                purchaseService,
                                                                                permissionService)
        {
            this.organisationService = organisationService;
            this.userDataService = userDataService;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await LoadAmphoraAsync(id);
            await SetPagePropertiesAsync();
            await LoadUserData();
            TryLoadPurchase();
            return OnReturnPage();
        }

        public async Task LoadUserData()
        {
            var userDataRes = await userDataService.ReadAsync(User);
            if (userDataRes.Succeeded)
            {
                this.UserData = userDataRes.Entity;
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

            return RedirectToPage("./Options", new { Id = id });
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
                    return RedirectToPage("./Options", new { Id = Amphora.Id });
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
                    return RedirectToPage("./Options", new { Id = Amphora.Id });
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
                    var res = await userDataService.UpdateAsync(User, UserData);
                    return RedirectToPage("./Options", new { Id = Amphora.Id });
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
                    return RedirectToPage("./Options", new { Id = Amphora.Id });
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "No free places to pin");
            }

            return OnReturnPage();
        }
    }
}