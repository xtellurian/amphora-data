using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Admin.Pages.Accounts
{
    [GlobalAdminAuthorize]
    public class DetailPageModel : PageModel
    {
        private readonly IEntityStore<OrganisationModel> orgStore;

        public DetailPageModel(IEntityStore<OrganisationModel> orgStore)
        {
            this.orgStore = orgStore;
        }

        public OrganisationModel Org { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Org = await orgStore.ReadAsync(id);
            return Page();
        }
    }
}