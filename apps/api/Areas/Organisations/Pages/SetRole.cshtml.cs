using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages
{
    public class SetRoleModel : PageModel
    {
        private readonly IOrganisationService organisationService;
        private readonly IPermissionService permissionService;
        private readonly IUserService userService;

        public SetRoleModel(IOrganisationService organisationService, IPermissionService permissionService, IUserService userService)
        {
            this.organisationService = organisationService;
            this.permissionService = permissionService;
            this.userService = userService;
        }

        public OrganisationModel Organisation { get; private set; }
        public Membership TargetMembership { get; private set; }
        public bool Succeeded { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id, string userId, string role)
        {
            if (role == "admin")
            {
                this.Organisation = await organisationService.Store.ReadAsync(id);
                this.TargetMembership = Organisation.Memberships.FirstOrDefault(m => m.UserId == userId);
                if (TargetMembership == null) { return RedirectToPage("./Members"); }
                // this section is a fix for bug 382 >>
                var user = await userService.ReadUserModelAsync(User);
                var authorized = await permissionService.IsAuthorizedAsync(user, Organisation, Common.Models.Permissions.AccessLevels.Administer);
                if (!authorized)
                {
                    ModelState.AddModelError(string.Empty, "Unauthorised");
                    return Page();
                }

                // << end fix
                TargetMembership.Role = Common.Models.Organisations.Roles.Administrator;
                var res = await organisationService.UpdateAsync(User, Organisation);
                if (res.Succeeded)
                {
                    this.Succeeded = true;
                    return Page();
                }
                else if (res.WasForbidden)
                {
                    ModelState.AddModelError(string.Empty, "Permission Denied");
                    return Page();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, res.Message);
                    return Page();
                }
            }
            else
            {
                throw new System.NotImplementedException($"Cannot set role to {role}");
            }
        }
    }
}