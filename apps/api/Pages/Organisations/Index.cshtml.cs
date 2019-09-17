using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Organisations
{
    [Authorize]
    public class IndexModel: PageModel
    {
        private readonly IEntityStore<OrganisationModel> entityStore;

        public IndexModel(IEntityStore<OrganisationModel> entityStore)
        {
            this.entityStore = entityStore;
        }

        public IList<OrganisationModel> Orgs {get; set;}

        public async Task<IActionResult> OnGetAsync()
        {
            Orgs = await entityStore.TopAsync();
            return Page();
        }
    }
}