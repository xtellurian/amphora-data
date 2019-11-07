using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Admin.Pages.Accounts
{
    [GlobalAdminAuthorize]
    public class IndexPageModel : PageModel
    {
        private readonly IEntityStore<OrganisationModel> orgStore;
        private readonly IAccountsService accountsService;

        public IndexPageModel(IEntityStore<OrganisationModel> orgStore, IAccountsService accountsService)
        {
            this.orgStore = orgStore;
            this.accountsService = accountsService;
        }

        public IList<OrganisationModel> Orgs { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Orgs = await orgStore.TopAsync();
            return Page();
        }
    }
}