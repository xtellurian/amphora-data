using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Amphorae
{
    public class IndexModel : PageModel
    {
        private readonly IOrgEntityStore<Common.Models.Amphora> amphoraEntityStore;
        private readonly IDataStore<Common.Models.Amphora, byte[]> dataStore;
        private readonly IMapper mapper;

        public IndexModel(
            IOrgEntityStore<Amphora.Common.Models.Amphora> amphoraEntityStore,
            IDataStore<Amphora.Common.Models.Amphora, byte[]> dataStore,
            IMapper mapper)
        {
            this.amphoraEntityStore = amphoraEntityStore;
            this.dataStore = dataStore;
            this.mapper = mapper;
            this.Amphorae = new List<Amphora.Common.Models.Amphora>();
        }

        [BindProperty]
        public IEnumerable<Amphora.Common.Models.Amphora> Amphorae { get; set; }

        public async Task<IActionResult> OnGetAsync(string orgId)
        {
            if (orgId != null)
            {
                this.Amphorae = await amphoraEntityStore.ListAsync(orgId);
            }
            else
            {
                this.Amphorae = await amphoraEntityStore.ListAsync();
            }
            
            return Page();
        }
    }
}