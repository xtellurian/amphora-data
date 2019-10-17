using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages
{
    public class MembersModel: PageModel
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
            if(Organisation == null) return RedirectToPage("./Index");
            
            return Page();
        }
    }
}