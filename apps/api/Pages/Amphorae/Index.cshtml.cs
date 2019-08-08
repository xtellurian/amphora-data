using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Amphorae
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IOrgEntityStore<Common.Models.Amphora> amphoraEntityStore;
        private readonly IDataStore<Common.Models.Amphora, byte[]> dataStore;
        private readonly IMapper mapper;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            IOrgEntityStore<Amphora.Common.Models.Amphora> amphoraEntityStore,
            IDataStore<Amphora.Common.Models.Amphora, byte[]> dataStore,
            IMapper mapper)
        {
            this.userManager = userManager;
            this.amphoraEntityStore = amphoraEntityStore;
            this.dataStore = dataStore;
            this.mapper = mapper;
            this.Amphorae = new List<Amphora.Common.Models.Amphora>();
        }

        [BindProperty]
        public IEnumerable<Amphora.Common.Models.Amphora> Amphorae { get; set; }

        public async Task<IActionResult> OnGetAsync(string orgId)
        {
            var user = await this.userManager.GetUserAsync(User);
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