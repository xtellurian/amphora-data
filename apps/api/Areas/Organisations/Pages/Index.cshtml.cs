using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Amphora.Api.Areas.Organisations.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IEntityStore<OrganisationModel> entityStore;
        [BindProperty(SupportsGet = true)]
        public string Name { get; set; }

        public IndexModel(IEntityStore<OrganisationModel> entityStore)
        {
            this.entityStore = entityStore;
        }

        public IList<OrganisationModel> Orgs { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Name))
            {
                Orgs = await entityStore.TopAsync();
            }
            else
            {
                var lowerName = Name.ToLower();
                Orgs = (await entityStore.QueryAsync(_ => true)).Where(o => o.Name.ToLower().Contains(Name.ToLower())).ToList();
            }

            return Page();
        }
    }
}