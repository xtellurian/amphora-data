using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Permissions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Areas.Amphorae.Pages.Signals
{
    [CommonAuthorize]
    public class DeleteModel : AmphoraPageModel
    {
        private readonly IPermissionService permissionService;
        private readonly IUserDataService userDataService;
        private readonly ILogger<DeleteModel> logger;

        public DeleteModel(IAmphoraeService amphoraeService,
                           IPermissionService permissionService,
                           IUserDataService userDataService,
                           ILogger<DeleteModel> logger) : base(amphoraeService)
        {
            this.permissionService = permissionService;
            this.userDataService = userDataService;
            this.logger = logger;
        }

        public SignalV2 Signal { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id, string signalId)
        {
            await LoadAmphoraAsync(id);

            if (Result.Succeeded)
            {
                Signal = Amphora.V2Signals.FirstOrDefault(a => a.Id == signalId);
                if (Signal == null)
                {
                    ModelState.AddModelError(string.Empty, $"Signal {signalId} not found");
                    return Page();
                }

                var userReadRes = await userDataService.ReadAsync(User);
                if (!userReadRes.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, userReadRes.Message);
                    return Page();
                }

                var authorized = await permissionService.IsAuthorizedAsync(userReadRes.Entity, Amphora, AccessLevels.Update);
                if (!authorized)
                {
                    ModelState.AddModelError(string.Empty, "Delete permission denied");
                }

                return Page();
            }
            else
            {
                return RedirectToPage("/Index/Signals");
            }
        }

        public async Task<IActionResult> OnPostAsync(string id, string signalId)
        {
            await LoadAmphoraAsync(id);
            if (Result.Succeeded)
            {
                Signal = Amphora.V2Signals.FirstOrDefault(a => a.Id == signalId);
                if (Signal == null)
                {
                    ModelState.AddModelError(string.Empty, $"Signal {signalId} not found");
                    return Page();
                }

                Amphora.V2Signals.Remove(Signal);
                var result = await amphoraeService.UpdateAsync(User, Amphora);
                if (result.Succeeded)
                {
                    return RedirectToPage("/Detail/Signals", new { Id = Amphora.Id });
                }
                else if (result.WasForbidden)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                    return Page();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                    return Page();
                }
            }

            return RedirectToPage("/Detail/Signals", new { Id = Amphora.Id });
        }
    }
}