using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Amphorae.Pages.Files
{
    [Authorize]
    public class DeletePageModel : PageModel
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IBlobStore<AmphoraModel> blobStore;
        private readonly IPermissionService permissionService;
        private readonly IUserService userService;

        public DeletePageModel(
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
        public string Name { get; private set; }
        public AmphoraModel Amphora { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id, string name)
        {
            if (string.IsNullOrEmpty(name)) { return RedirectToPage("../Detail", new { Id = id }); }
            await LoadProperties(id, name);
            if (Amphora == null)
            {
                return RedirectToPage("../Index");
            }

            var user = await userService.UserManager.GetUserAsync(User);
            if (await permissionService.IsAuthorizedAsync(user, Amphora, Common.Models.Permissions.AccessLevels.Update))
            {
                return Page();
            }
            else
            {
                return RedirectToPage("../Forbidden");
            }
        }

        public async Task<IActionResult> OnPostAsync(string id, string name)
        {
            if (string.IsNullOrEmpty(name)) { return RedirectToPage("./Detail", new { Id = id }); }
            await LoadProperties(id, name);
            if (Amphora == null)
            {
                return RedirectToPage("../Index");
            }

            var user = await userService.UserManager.GetUserAsync(User);
            if (await permissionService.IsAuthorizedAsync(user, Amphora, Common.Models.Permissions.AccessLevels.Update))
            {
                await blobStore.DeleteAsync(Amphora, name);
                return RedirectToPage("./Index", new { id = id });
            }
            else
            {
                return RedirectToPage("../Forbidden");
            }
        }

        private async Task LoadProperties(string id, string name)
        {
            this.Amphora = await amphoraeService.AmphoraStore.ReadAsync(id);
            this.Name = name;
        }
    }
}