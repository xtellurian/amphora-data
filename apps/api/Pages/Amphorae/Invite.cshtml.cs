using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Permissions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Amphorae
{
    public class InviteModel : PageModel
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IPermissionService permissionService;
        private readonly IUserService userService;

        public InviteModel(
            IAmphoraeService amphoraeService,
            IPermissionService permissionService,
            IUserService userService)
        {
            this.amphoraeService = amphoraeService;
            this.permissionService = permissionService;
            this.userService = userService;
        }
        public class InputModel
        {
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public AmphoraModel Amphora { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await userService.UserManager.GetUserAsync(User);
            var result = await amphoraeService.ReadAsync(User, id);

            if (result.Succeeded)
            {
                this.Amphora = result.Entity;
                var authorized = await permissionService.IsAuthorizedAsync(user, Amphora, AccessLevels.Administer);
                if (!authorized)
                {
                    this.ModelState.AddModelError(string.Empty, "Unauthoirized");
                }
                return Page();
            }
            else if (result.WasForbidden)
            {
                return RedirectToPage("./Forbidden");
            }
            else
            {
                return RedirectToPage("./Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                ModelState.AddModelError(string.Empty, "Not Found");
                return Page();
            }
            var user = await userService.UserManager.GetUserAsync(User);
            var result = await amphoraeService.ReadAsync(User, id);
            var authorized = await permissionService.IsAuthorizedAsync(user, result.Entity, AccessLevels.Administer);
            if (authorized && result.Succeeded)
            {
                Amphora = result.Entity;
                var toInvite = await userService.UserManager.FindByNameAsync(Input.Email);
                if (toInvite == null)
                {
                    ModelState.AddModelError(string.Empty, $"{Input.Email} is not an Amphora Data user");
                    return Page();
                }
                else
                {
                    await permissionService.UpdatePermissionAsync(toInvite, Amphora, AccessLevels.ReadContents);
                    return RedirectToPage("./Detail", new { id = Amphora.AmphoraId });
                }
            }
            else if (result.WasForbidden)
            {
                return RedirectToPage("./Forbidden");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Unauthorized!");
                ModelState.AddModelError(string.Empty, result.Message);
                return Page();
            }
        }
    }
}