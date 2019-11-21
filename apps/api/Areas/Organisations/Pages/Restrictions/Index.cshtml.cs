using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages.Restrictions
{
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
            var readRes = await organisationService.ReadAsync(User, id);
            if (id == null)
            {
                return RedirectToPage("./Index", new { Id = readRes.User.OrganisationId }); // reload page
            }
            if (readRes.Succeeded)
            {
                this.Organisation = readRes.Entity;
                return Page();
            }
            else
            {
                return StatusCode(403);
            }
        }
    }
}