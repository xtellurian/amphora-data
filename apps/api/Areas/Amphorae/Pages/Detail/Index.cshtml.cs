using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages.Detail
{
    public class IndexPageModel : AmphoraDetailPageModel
    {
        public IndexPageModel(IAmphoraeService amphoraeService,
                              IQualityEstimatorService qualityEstimator,
                              IPurchaseService purchaseService,
                              IPermissionService permissionService) : base(amphoraeService, qualityEstimator, purchaseService, permissionService)
        {
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await LoadAmphoraAsync(id);
            await SetPagePropertiesAsync();
            TryLoadPurchase();
            return OnReturnPage();
        }
    }
}