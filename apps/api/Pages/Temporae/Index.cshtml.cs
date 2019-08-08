using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;

namespace Amphora.Api.Pages.Temporae
{
    public class IndexModel : PageModel
    {
        private readonly IOrgEntityStore<Common.Models.Tempora> temporaEntityStore;

        public IndexModel(
            IOrgEntityStore<Amphora.Common.Models.Tempora> temporaEntityStore)
        {
            this.temporaEntityStore = temporaEntityStore;
            this.Temporae = new List<Amphora.Common.Models.Tempora>();
        }

        [BindProperty]
        public IEnumerable<Amphora.Common.Models.Tempora> Temporae { get; set; }

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