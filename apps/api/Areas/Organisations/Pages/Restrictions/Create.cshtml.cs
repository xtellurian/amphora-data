using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Permissions;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Permissions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Amphora.Api.Areas.Organisations.Pages.Restrictions
{
    public class CreateModel : PageModel
    {
        private readonly IOrganisationService organisationService;

        public CreateModel(IOrganisationService organisationService)
        {
            this.organisationService = organisationService;
        }

        public SelectList Kinds = Selectlists.EnumSelectlist<RestrictionKind>(true);
        [BindProperty]
        public Restriction NewRestriction { get; set; } = new Restriction();
        public OrganisationModel Organisation { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id, string targetOrganisationId = null)
        {
            var readRes = await organisationService.ReadAsync(User, id);
            if (id == null)
            {
                return RedirectToPage("./Create", new { id = readRes.User.OrganisationId, targetOrganisationId = targetOrganisationId }); // reload page
            }

            this.NewRestriction.TargetOrganisationId = targetOrganisationId;
            if (readRes.Succeeded)
            {
                this.Organisation = readRes.Entity;
                return Page();
            }
            else if (readRes.WasForbidden) { return StatusCode(403); }
            else { return RedirectToPage("/Detail", new { Area = "Organisations", Id = id }); }
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var readRes = await organisationService.ReadAsync(User, id);
            if (readRes.Succeeded)
            {
                this.Organisation = readRes.Entity;
                if (!await IsTargetReal())
                {
                    ModelState.AddModelError(string.Empty, "Organisation with that Id doesn't exist");
                    return Page();
                }

                var restriction = new RestrictionModel(NewRestriction.TargetOrganisationId)
                {
                    Kind = NewRestriction.Kind,
                    SourceOrganisationId = Organisation.Id
                };
                this.Organisation.Restrictions.Add(restriction);
                var updateRes = await organisationService.UpdateAsync(User, Organisation);
                if (updateRes.Succeeded) { return RedirectToPage("./Index", new { Id = Organisation.Id }); }
                else if (updateRes.WasForbidden) { return StatusCode(403); }
                else
                {
                    ModelState.AddModelError(string.Empty, updateRes.Message);
                    return Page();
                }
            }
            else if (readRes.WasForbidden) { return StatusCode(403); }
            else { return RedirectToPage("/Detail", new { Area = "Organisations", Id = id }); }
        }

        private async Task<bool> IsTargetReal()
        {
            var org = await organisationService.Store.ReadAsync(NewRestriction.TargetOrganisationId);
            return org != null;
        }
    }
}