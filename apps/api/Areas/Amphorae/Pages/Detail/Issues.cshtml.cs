using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages.Detail
{
    [CommonAuthorize]
    public class IssuesPageModel : AmphoraDetailPageModel
    {
        public IssuesPageModel(IAmphoraeService amphoraeService,
                                   IQualityEstimatorService qualityEstimator,
                                   IPurchaseService purchaseService,
                                   IPermissionService permissionService) : base(amphoraeService,
                                                                                qualityEstimator,
                                                                                purchaseService,
                                                                                permissionService)
        {
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await LoadAmphoraAsync(id);
            await SetPagePropertiesAsync();
            return OnReturnPage();
        }
    }
}