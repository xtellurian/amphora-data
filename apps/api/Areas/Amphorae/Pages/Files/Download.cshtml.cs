using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Amphorae.Pages.Files
{
    [Authorize]
    public class DownloadPageModel : PageModel
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IBlobStore<AmphoraModel> blobStore;
        private readonly IPermissionService permissionService;
        private readonly IUserService userService;

        public DownloadPageModel(
            IAmphoraeService amphoraeService,
            IBlobStore<AmphoraModel> blobStore,
            IPermissionService permissionService,
            IUserService userService)
        {
            this.amphoraeService = amphoraeService;
            this.blobStore = blobStore;
            this.permissionService = permissionService;
            this.userService = userService;
        }

        public bool Succeeded { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id, string name)
        {
            if (string.IsNullOrEmpty(name)) { return RedirectToPage("./Detail", new { Id = id }); }
            var entity = await amphoraeService.AmphoraStore.ReadAsync(id);
            if (entity == null)
            {
                return RedirectToPage("./Index");
            }

            var user = await userService.UserManager.GetUserAsync(User);
            if (await permissionService.IsAuthorizedAsync(user, entity, Common.Models.Permissions.AccessLevels.ReadContents))
            {
                var file = await blobStore.ReadBytesAsync(entity, name);
                if (file == null || file.Length == 0)
                {
                    ModelState.AddModelError(string.Empty, "Uh Oh, this file appears to be empty.");
                    this.Succeeded = false;
                    return Page();
                }

                return File(file, "application/octet-stream", name);
            }
            else
            {
                return RedirectToPage("./Forbidden");
            }
        }
    }
}