using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Temporae
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IOrgScopedEntityStore<Common.Models.Amphora> temporaEntityStore;

        public IndexModel(
            IOrgScopedEntityStore<Amphora.Common.Models.Amphora> temporaEntityStore)
        {
            this.temporaEntityStore = temporaEntityStore;
            this.Temporae = new List<Amphora.Common.Models.Amphora>();
        }

        [BindProperty]
        public IEnumerable<Amphora.Common.Models.Amphora> Temporae { get; set; }

        public async Task<IActionResult> OnGetAsync(string orgId)
        {
            if (orgId != null)
            {
                this.Temporae = await temporaEntityStore.ListAsync(orgId);
            }
            else
            {
                this.Temporae = await temporaEntityStore.ListAsync();
            }
            
            return Page();
        }
    }
}