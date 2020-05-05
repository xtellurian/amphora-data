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
            return OnReturnPage();
        }

        protected override async Task SetPagePropertiesAsync()
        {
            await base.SetPagePropertiesAsync();
            await LoadUserData();
        }

        private async Task LoadUserData()
        {
            var userDataRes = await userDataService.ReadAsync(User);
            if (userDataRes.Succeeded)
            {
                this.UserData = userDataRes.Entity;
            }
        }
    }
}