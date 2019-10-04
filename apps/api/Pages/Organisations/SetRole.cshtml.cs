using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Organisations
{
    public class SetRoleModel : PageModel
    {
        private readonly IOrganisationService organisationService;

        public SetRoleModel(IOrganisationService organisationService)
        {
            this.organisationService = organisationService;
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
                if (TargetMembership == null) return RedirectToPage("./Members");
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