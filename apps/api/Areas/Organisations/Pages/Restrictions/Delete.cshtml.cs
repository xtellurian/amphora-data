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

        public DeleteModel(IOrganisationService organisationService)
        {
            this.organisationService = organisationService;
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

            if(readRes.Succeeded)
            {
                this.Organisation = readRes.Entity;
                this.Target = this.Organisation.Restrictions.FirstOrDefault(_ => _.TargetOrganisationId == targetOrganisationId);
                if(Target == null)
                {
                    return RedirectToDetail(id);
                }
                return Page();
            }
            else if(readRes.WasForbidden) return StatusCode(403);
            else return RedirectToDetail(id);
        }

        public async Task<IActionResult> OnPostAsync(string id, string targetOrganisationId )
        {
            var readRes = await organisationService.ReadAsync(User, id);
            if (id == null)
            {
                return RedirectToPage("./Delete", new { id = readRes.User.OrganisationId, targetOrganisationId = targetOrganisationId }); // reload page
            }

            if(readRes.Succeeded)
            {
                this.Organisation = readRes.Entity;
                this.Target = this.Organisation.Restrictions.FirstOrDefault(_ => _.TargetOrganisationId == targetOrganisationId);
                if(Target == null)
                {
                    return RedirectToDetail(id);
                }

                this.Organisation.Restrictions.Remove(this.Target);
                var updateRes = await organisationService.UpdateAsync(User, Organisation);
                if(updateRes.Succeeded) return RedirectToDetail(id);
                else if(updateRes.WasForbidden) return StatusCode(403);
                else ModelState.AddModelError(string.Empty, updateRes.Message);
                return Page();
            }
            else if(readRes.WasForbidden) return StatusCode(403);
            else return RedirectToDetail(id);
        }

        private IActionResult RedirectToDetail(string id)
        {
            return RedirectToPage("/Detail", new { Area = "Organisations", Id = id }); 
        }
    }
}