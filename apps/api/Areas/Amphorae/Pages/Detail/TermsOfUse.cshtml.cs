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
        private readonly ITermsOfUseService termsOfUseService;
        private readonly IOrganisationService organisationService;

        public TermsOfUsePageModel(IAmphoraeService amphoraeService,
                                   IQualityEstimatorService qualityEstimator,
                                   IPurchaseService purchaseService,
                                   IPermissionService permissionService,
                                   ITermsOfUseService termsOfUseService,
                                   IOrganisationService organisationService) : base(amphoraeService,
                                                                                qualityEstimator,
                                                                                purchaseService,
                                                                                permissionService)
        {
            this.termsOfUseService = termsOfUseService;
            this.organisationService = organisationService;
        }

        public bool PromptAccept { get; private set; }

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
                var tou = Amphora.TermsOfUse;
                if (tou != null)
                {
                    var acceptRes = await termsOfUseService.AcceptAsync(User, tou);
                    if (acceptRes.Succeeded)
                    {
                        await SetPagePropertiesAsync();
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error Accepting those Terms");
                }
            }

            return OnReturnPage();
        }

        protected override async Task SetPagePropertiesAsync()
        {
            await base.SetPagePropertiesAsync();
        }
    }
}