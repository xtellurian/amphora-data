using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Permissions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Amphora.Api.Areas.Amphorae.Pages
{
    [CommonAuthorize]
    public class InviteModel : PageModel
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IPermissionService permissionService;
        private readonly IMapper mapper;
        private readonly IUserDataService userDataService;

        public InviteModel(
            IAmphoraeService amphoraeService,
            IPermissionService permissionService,
            IMapper mapper,
            IUserDataService userDataService)
        {
            this.amphoraeService = amphoraeService;
            this.permissionService = permissionService;
            this.mapper = mapper;
            this.userDataService = userDataService;
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
            var userReadRes = await userDataService.ReadAsync(User);
            var result = await amphoraeService.ReadAsync(User, id);

            if (result.Succeeded && userReadRes.Succeeded)
            {
                this.Amphora = result.Entity;
                var authorized = await permissionService.IsAuthorizedAsync(userReadRes.Entity, Amphora, AccessLevels.Administer);
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

            var userReadRes = await userDataService.ReadAsync(User);
            var result = await amphoraeService.ReadAsync(User, id);

            var authorized = await permissionService.IsAuthorizedAsync(userReadRes.Entity, result.Entity, AccessLevels.Administer);
            if (authorized && result.Succeeded)
            {
                Amphora = result.Entity;
                var toInvite = await userDataService.Query(User, _ => _.ContactInformation.Email == Input.Email).ToListAsync();
                if (toInvite == null || toInvite.Count == 0)
                {
                    ModelState.AddModelError(string.Empty, $"{Input.Email} is not an Amphora Data user");
                    return Page();
                }
                else
                {
                    var securityModel = mapper.Map<AmphoraModel>(result.Entity);
                    await amphoraeService.AmphoraStore.UpdateAsync(securityModel);
                    return RedirectToPage("./Detail", new { id = Amphora.Id });
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