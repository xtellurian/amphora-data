using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Organisations
{
    [Authorize]
    public class IndexModel: PageModel
    {
        private readonly IEntityStore<Organisation> entityStore;

        public IndexModel(IEntityStore<Organisation> entityStore)
        {
            this.entityStore = entityStore;
        }

        public IList<Organisation> Orgs {get; set;}

        public async Task<IActionResult> OnGetAsync()
        {
            Orgs = await entityStore.ListAsync();
            return Page();
        }
    }
}