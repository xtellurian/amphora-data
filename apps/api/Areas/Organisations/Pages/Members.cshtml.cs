using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages
{
    [CommonAuthorize]
    public class MembersModel : PageModel
    {
        private readonly IOrganisationService orgService;

        public MembersModel(IOrganisationService orgService)
        {
            this.orgService = orgService;
        }

        public OrganisationModel Organisation { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            this.Organisation = await orgService.Store.ReadAsync(id);
            if (Organisation == null)
            {
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}