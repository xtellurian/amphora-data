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
    public class DeletePageModel : PageModel
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IAmphoraFileService amphoraFileService;
        private readonly IPermissionService permissionService;
        private readonly IUserDataService userDataService;

        public DeletePageModel(
            IAmphoraeService amphoraeService,
            IAmphoraFileService amphoraFileService,
            IPermissionService permissionService,
            IUserDataService userDataService)
        {
            this.amphoraeService = amphoraeService;
            this.amphoraFileService = amphoraFileService;
            this.permissionService = permissionService;
            this.userDataService = userDataService;
        }

        public bool Succeeded { get; private set; }
        public string Name { get; private set; }
        public AmphoraModel Amphora { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id, string name)
        {
            if (string.IsNullOrEmpty(name)) { return RedirectToPage("/Detail/Index", new { Id = id }); }
            await LoadProperties(id, name);
            if (Amphora == null)
            {
                return RedirectToPage("/Index");
            }

            var userReadRes = await userDataService.ReadAsync(User);
            if (userReadRes.Succeeded)
            {
                var user = userReadRes.Entity;
                if (await permissionService.IsAuthorizedAsync(user, Amphora, Common.Models.Permissions.AccessLevels.Update))
                {
                    return Page();
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

        public async Task<IActionResult> OnPostAsync(string id, string name)
        {
            if (string.IsNullOrEmpty(name)) { return RedirectToPage("/Detail/Index", new { Id = id }); }
            await LoadProperties(id, name);
            if (Amphora == null)
            {
                return RedirectToPage("/Index");
            }

            var deleteRes = await amphoraFileService.DeleteFileAsync(User, Amphora, name);
            if (deleteRes.Succeeded)
            {
                return RedirectToPage("/Detail/Files", new { id = id });
            }
            else
            {
                return RedirectToPage("/Forbidden");
            }
        }

        private async Task LoadProperties(string id, string name)
        {
            this.Amphora = await amphoraeService.AmphoraStore.ReadAsync(id);
            this.Name = name;
        }
    }
}