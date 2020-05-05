using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages.TermsOfUse
{
    [CommonAuthorize]
    public class IndexModel : PageModel
    {
        private readonly IOrganisationService organisationService;
        private readonly IUserDataService userDataService;

        public IndexModel(IOrganisationService organisationService, IUserDataService userDataService)
        {
            this.organisationService = organisationService;
            this.userDataService = userDataService;
        }

        public OrganisationModel Organisation { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var userDataRes = await userDataService.ReadAsync(User);
            var result = await organisationService.ReadAsync(User, id);
            if (result.Succeeded)
            {
                this.Organisation = result.Entity;
                return Page();
            }
            else if (userDataRes.Succeeded)
            {
                this.Organisation = userDataRes.Entity.Organisation;
                return Page();
            }
            else
            {
                return RedirectToPage("/Index");
            }
        }
    }
}