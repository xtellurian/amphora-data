using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages.Signals
{
    [Authorize]
    public class IndexModel : AmphoraPageModel
    {
        private readonly IPermissionService permissionService;

        public IndexModel(IAmphoraeService amphoraeService, IPermissionService permissionService) : base(amphoraeService)
        {
            this.permissionService = permissionService;
        }

        public IEnumerable<SignalV2> Signals { get; private set; }
        public bool CanDeleteProperty { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await LoadAmphoraAsync(id);

            if (Amphora != null)
            {
                this.Signals = this.Amphora.V2Signals;
                this.CanDeleteProperty = await permissionService.IsAuthorizedAsync(Result.User, Amphora, Common.Models.Permissions.AccessLevels.Update);
            }

            return OnReturnPage();
        }
    }
}
