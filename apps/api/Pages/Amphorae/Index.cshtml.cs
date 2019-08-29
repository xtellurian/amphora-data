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
        private readonly IUserManager<ApplicationUser> userManager;
        private readonly IOrgScopedEntityStore<Common.Models.Amphora> entityStore;
        private readonly IDataStore<Common.Models.Amphora, byte[]> dataStore;
        private readonly IMapper mapper;

        public IndexModel(
            IUserManager<ApplicationUser> userManager,
            IOrgScopedEntityStore<Amphora.Common.Models.Amphora> entityStore,
            IDataStore<Amphora.Common.Models.Amphora, byte[]> dataStore,
            IMapper mapper)
        {
            this.userManager = userManager;
            this.entityStore = entityStore;
            this.dataStore = dataStore;
            this.mapper = mapper;
            this.Amphorae = new List<Amphora.Common.Models.Amphora>();
        }

        [BindProperty]
        public IEnumerable<Amphora.Common.Models.Amphora> Amphorae { get; set; }

        public async Task<IActionResult> OnGetAsync(string orgId, string geoHash)
        {
            var user = await this.userManager.GetUserAsync(User);

            if(! string.IsNullOrEmpty(geoHash))
            {
                this.Amphorae = await this.entityStore.StartsWithQueryAsync("GeoHash", geoHash);
                return Page();
            }
            if (orgId != null)
            {
                this.Amphorae = await entityStore.ListAsync(orgId);
            }
            else
            {
                this.Amphorae = await entityStore.ListAsync();
            }
            
            return Page();
        }
    }
}