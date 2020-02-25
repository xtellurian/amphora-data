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
        private readonly IRestrictionService restrictionService;

        public CreateModel(IOrganisationService organisationService, IRestrictionService restrictionService)
        {
            this.organisationService = organisationService;
            this.restrictionService = restrictionService;
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
                var targetOrg = await organisationService.Store.ReadAsync(NewRestriction.TargetOrganisationId);
                if (targetOrg == null)
                {
                    ModelState.AddModelError(string.Empty, "Organisation with that Id doesn't exist");
                    return Page();
                }

                var restriction = new RestrictionModel(readRes.Entity, targetOrg, NewRestriction.Kind);

                var createRes = await restrictionService.CreateAsync(User, restriction);
                if (createRes.Succeeded) { return RedirectToPage("./Index", new { Id = Organisation.Id }); }
                else if (createRes.WasForbidden) { return StatusCode(403); }
                else
                {
                    ModelState.AddModelError(string.Empty, createRes.Message);
                    return Page();
                }
            }
            else if (readRes.WasForbidden) { return StatusCode(403); }
            else { return RedirectToPage("/Detail", new { Area = "Organisations", Id = id }); }
        }
    }
}