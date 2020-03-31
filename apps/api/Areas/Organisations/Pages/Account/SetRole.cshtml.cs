using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Organisations.Pages
{
    public class SetRolePageModel : OrganisationPageModel
    {
        private readonly IPermissionService permissionService;

        public SetRolePageModel(IOrganisationService organisationService,
                                IPermissionService permissionService,
                                IUserDataService userDataService) : base(organisationService, userDataService)
        {
            this.permissionService = permissionService;
        }

        public Membership TargetMembership { get; private set; }
        public bool Succeeded { get; private set; }

        public async Task<IActionResult> OnGetAsync(string userId, string role)
        {
            await LoadPropertiesAsync();
            if (role == "admin")
            {
                this.TargetMembership = Organisation.Memberships.FirstOrDefault(m => m.UserId == userId);
                if (TargetMembership == null) { return RedirectToPage("./Members"); }
                // this section is a fix for bug 382 >>
                var userReadRes = await userDataService.ReadAsync(User);
                if (!userReadRes.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, userReadRes.Message);
                    return Page();
                }

                var userData = userReadRes.Entity;
                var authorized = await permissionService.IsAuthorizedAsync(userData, Organisation, Common.Models.Permissions.AccessLevels.Administer);
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