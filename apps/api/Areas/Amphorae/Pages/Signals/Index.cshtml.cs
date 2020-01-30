using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Signals;
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

        public IEnumerable<SignalModel> Signals { get; private set; }
        public bool CanDeleteProperty { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await LoadAmphoraAsync(id);

            if (Amphora != null)
            {
                this.Signals = this.Amphora.Signals.Select(s => s.Signal);
                this.CanDeleteProperty = await permissionService.IsAuthorizedAsync(Result.User, Amphora, Common.Models.Permissions.AccessLevels.Update);
            }

            return OnReturnPage();
        }
    }
}
