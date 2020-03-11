using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages.TermsAndConditions
{
    [CommonAuthorize]
    public class IndexModel : PageModel
    {
        private readonly IOrganisationService organisationService;

        public IndexModel(IOrganisationService organisationService)
        {
            this.organisationService = organisationService;
        }

        public OrganisationModel Organisation { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var result = await organisationService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                this.Organisation = result.Entity;
                return Page();
            }
            else
            {
                return RedirectToPage("/Index");
            }
        }
    }
}