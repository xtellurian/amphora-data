using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Permissions;
using Amphora.Common.Models.Signals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Areas.Amphorae.Pages.Signals
{
    [Authorize]
    public class DeleteModel : AmphoraPageModel
    {
        private readonly IPermissionService permissionService;
        private readonly IUserService userService;
        private readonly ILogger<DeleteModel> logger;

        public DeleteModel(IAmphoraeService amphoraeService,
                           IPermissionService permissionService,
                           IUserService userService,
                           ILogger<DeleteModel> logger) : base(amphoraeService)
        {
            this.permissionService = permissionService;
            this.userService = userService;
            this.logger = logger;
        }

        public AmphoraSignalModel Signal { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id, string signalId)
        {
            await LoadAmphoraAsync(id);

            if (Result.Succeeded)
            {
                Signal = Amphora.Signals.FirstOrDefault(a => a.SignalId == signalId);
                if (Signal == null)
                {
                    ModelState.AddModelError(string.Empty, $"Signal {signalId} not found");
                    return Page();
                }

                var user = await userService.ReadUserModelAsync(User);
                var authorized = await permissionService.IsAuthorizedAsync(user, Amphora, AccessLevels.Update);
                if (!authorized)
                {
                    ModelState.AddModelError(string.Empty, "Delete permission denied");
                }

                return Page();
            }
            else
            {
                return RedirectToPage("./Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(string id, string signalId)
        {
            await LoadAmphoraAsync(id);
            if (Result.Succeeded)
            {
                Signal = Amphora.Signals.FirstOrDefault(a => a.SignalId == signalId);
                if (Signal == null)
                {
                    ModelState.AddModelError(string.Empty, $"Signal {signalId} not found");
                    return Page();
                }

                Amphora.Signals.Remove(Signal);
                var result = await amphoraeService.UpdateAsync(User, Amphora);
                if (result.Succeeded)
                {
                    return RedirectToPage("./Index", new { Id = Amphora.Id });
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

            return RedirectToPage("./Index", new { Id = Amphora.Id });
        }
    }
}