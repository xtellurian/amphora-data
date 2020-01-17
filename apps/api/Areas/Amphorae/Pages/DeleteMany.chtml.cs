using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Amphorae.Pages
{
    [Authorize]
    public class DeleteManyPageModel : PageModel
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IPermissionService permissionService;
        private ApplicationUser appUser;
        [TempData]
        public string DeleteMessage { get; set; }

        public DeleteManyPageModel(IAmphoraeService amphoraeService, IPermissionService permissionService)
        {
            this.amphoraeService = amphoraeService;
            this.permissionService = permissionService;
        }

        public IList<AmphoraModel> Entities { get; set; } = new List<AmphoraModel>();

        public async Task<IActionResult> OnGetAsync(List<string> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                RedirectToPage("./Index");
            }

            DeleteMessage = null;
            await TryLoadAmphora(ids);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(List<string> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                RedirectToPage("./Index");
            }

            if (await TryLoadAmphora(ids))
            {
                foreach (var a in Entities)
                {
                    // first check we can delete (i.e. admin) all of them
                    if (!await permissionService.IsAuthorizedAsync(appUser, a, Common.Models.Permissions.AccessLevels.Administer))
                    {
                        this.ModelState.AddModelError(string.Empty, "Delete not authoried");
                        return Page();
                    }
                }

                foreach (var a in Entities)
                {
                    var deleteResult = await amphoraeService.DeleteAsync(User, a);
                    if (!deleteResult.Succeeded)
                    {
                        this.ModelState.AddModelError(string.Empty, "Uh Oh, something went wrong when deleting");
                        return Page();
                    }
                }

                this.Entities.Clear();
                this.DeleteMessage = $"Successfully deleted {Entities.Count} amphorae";
            }

            return Page();
        }

        private async Task<bool> TryLoadAmphora(IEnumerable<string> ids)
        {
            foreach (var id in ids)
            {
                var res = await amphoraeService.ReadAsync(User, id);
                appUser = res.User;
                if (res.Succeeded)
                {
                    Entities.Add(res.Entity);
                }
                else
                {
                    Entities.Clear();
                    ModelState.AddModelError(string.Empty, "You have insufficient permission");
                    return false;
                }
            }

            return true;
        }
    }
}