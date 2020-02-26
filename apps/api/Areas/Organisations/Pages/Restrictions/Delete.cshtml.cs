using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Permissions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages.Restrictions
{
    public class DeleteModel : PageModel
    {
        private readonly IOrganisationService organisationService;
        private readonly IRestrictionService restrictionService;

        public DeleteModel(IOrganisationService organisationService, IRestrictionService restrictionService)
        {
            this.organisationService = organisationService;
            this.restrictionService = restrictionService;
        }

        public OrganisationModel Organisation { get; private set; }
        public RestrictionModel Target { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id, string targetOrganisationId)
        {
            var readRes = await organisationService.ReadAsync(User, id);
            if (id == null)
            {
                return RedirectToPage("./Delete", new { id = readRes.User.OrganisationId, targetOrganisationId = targetOrganisationId }); // reload page
            }

            if (readRes.Succeeded)
            {
                this.Organisation = readRes.Entity;
                this.Target = this.Organisation.Restrictions.FirstOrDefault(_ => _.TargetOrganisationId == targetOrganisationId);
                if (Target == null)
                {
                    return RedirectToIndex(id);
                }

                return Page();
            }
            else if (readRes.WasForbidden) { return StatusCode(403); }
            else { return RedirectToIndex(id); }
        }

        public async Task<IActionResult> OnPostAsync(string id, string targetOrganisationId)
        {
            var readRes = await organisationService.ReadAsync(User, id);
            if (id == null)
            {
                return RedirectToPage("./Delete", new { id = readRes.User.OrganisationId, targetOrganisationId = targetOrganisationId }); // reload page
            }

            if (readRes.Succeeded)
            {
                this.Organisation = readRes.Entity;
                this.Target = this.Organisation.Restrictions.FirstOrDefault(_ => _.TargetOrganisationId == targetOrganisationId);
                if (Target == null)
                {
                    return RedirectToIndex(id);
                }

                var deleteRes = await restrictionService.DeleteAsync(User, Target.Id);
                if (deleteRes.Succeeded) { return RedirectToIndex(id); }
                else if (deleteRes.WasForbidden) { return StatusCode(403); }
                else { ModelState.AddModelError(string.Empty, deleteRes.Message); }
                return Page();
            }
            else if (readRes.WasForbidden) { return StatusCode(403); }
            else { return RedirectToIndex(id); }
        }

        private IActionResult RedirectToIndex(string id)
        {
            return RedirectToPage("/Restrictions/Index", new { Area = "Organisations", Id = id });
        }
    }
}