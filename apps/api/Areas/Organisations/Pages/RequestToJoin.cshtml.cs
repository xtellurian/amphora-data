using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages
{
    public class RequestToJoinPageModel : PageModel
    {
        private readonly IOrganisationService orgService;

        public RequestToJoinPageModel(IOrganisationService orgService)
        {
            this.orgService = orgService;
        }

        public OrganisationModel Organisation { get; private set; }
        public bool Succeeded { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            this.Organisation = await orgService.Store.ReadAsync(id);
            if (Organisation == null)
            {
                return RedirectToPage("./Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            this.Organisation = await orgService.Store.ReadAsync(id);
            if (Organisation == null)
            {
                return RedirectToPage("./Index");
            }

            // TODO: send join request
            return Page();
        }
    }
}