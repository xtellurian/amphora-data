using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages.Detail
{
    [CommonAuthorize]
    public class TermsOfUsePageModel : AmphoraDetailPageModel
    {
        private readonly IOrganisationService organisationService;

        public TermsOfUsePageModel(IAmphoraeService amphoraeService,
                                   IQualityEstimatorService qualityEstimator,
                                   IPurchaseService purchaseService,
                                   IPermissionService permissionService,
                                   IOrganisationService organisationService) : base(amphoraeService,
                                                                                qualityEstimator,
                                                                                purchaseService,
                                                                                permissionService)
        {
            this.organisationService = organisationService;
        }

        public bool PromptAccept { get; private set; }
        public bool HasAgreed { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id, bool promptAccept)
        {
            await LoadAmphoraAsync(id);
            await SetPagePropertiesAsync();
            this.PromptAccept = promptAccept;
            return OnReturnPage();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            await LoadAmphoraAsync(id);
            await SetPagePropertiesAsync();

            if (!HasAgreed)
            {
                var acceptRes = await organisationService.AgreeToTermsAndConditions(User, Amphora.TermsAndConditions);
                if (acceptRes.Succeeded)
                {
                    await SetPagePropertiesAsync();
                }
            }

            return OnReturnPage();
        }

        protected override async Task SetPagePropertiesAsync()
        {
            await base.SetPagePropertiesAsync();
            this.HasAgreed = await purchaseService.HasAgreedToTermsAndConditionsAsync(User, Amphora);
        }
    }
}