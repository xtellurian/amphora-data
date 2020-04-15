using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Amphorae.Pages.Files
{
    [CommonAuthorize]
    public class DownloadPageModel : PageModel
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IAmphoraBlobStore blobStore;
        private readonly IPermissionService permissionService;
        private readonly IUserDataService userDataService;

        public DownloadPageModel(
            IAmphoraeService amphoraeService,
            IAmphoraBlobStore blobStore,
            IPermissionService permissionService,
            IUserDataService userDataService)
        {
            this.amphoraeService = amphoraeService;
            this.blobStore = blobStore;
            this.permissionService = permissionService;
            this.userDataService = userDataService;
        }

        public bool Succeeded { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id, string name, bool redirect = true)
        {
            if (string.IsNullOrEmpty(name)) { return RedirectToPage("/Detail/Index", new { Id = id }); }
            var entity = await amphoraeService.AmphoraStore.ReadAsync(id);
            if (entity == null)
            {
                return RedirectToPage("/Index");
            }

            var userReadRes = await userDataService.ReadAsync(User);
            if (userReadRes.Succeeded)
            {
                var user = userReadRes.Entity;
                if (await permissionService.IsAuthorizedAsync(user, entity, Common.Models.Permissions.AccessLevels.ReadContents))
                {
                    if (!await blobStore.ExistsAsync(entity, name))
                    {
                        // file doesn't exist.
                        return NotFound();
                    }
                    else if (redirect)
                    {
                        var url = await blobStore.GetPublicUrl(entity, name);
                        return Redirect(url);
                    }
                    else
                    {
                        var file = await blobStore.ReadBytesAsync(entity, name);
                        if (file == null || file.Length == 0)
                        {
                            ModelState.AddModelError(string.Empty, "Uh Oh, this file appears to be empty.");
                            this.Succeeded = false;
                            return Page();
                        }

                        return File(file, ContentTypeRecogniser.GetContentType(name), name);
                    }
                }
                else
                {
                    return RedirectToPage("/Forbidden");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, userReadRes.Message);
                return Page();
            }
        }
    }
}