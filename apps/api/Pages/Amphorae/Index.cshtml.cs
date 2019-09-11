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
        private readonly IUserManager userManager;
        private readonly IAmphoraeService amphoraeService;
        private readonly IDataStore<Common.Models.AmphoraModel, byte[]> dataStore;
        private readonly IMapper mapper;

        public IndexModel(
            IUserManager userManager,
            IAmphoraeService amphoraeService,
            IEntityStore<Amphora.Common.Models.AmphoraModel> entityStore,
            IDataStore<Amphora.Common.Models.AmphoraModel, byte[]> dataStore,
            IMapper mapper)
        {
            this.userManager = userManager;
            this.amphoraeService = amphoraeService;
            this.dataStore = dataStore;
            this.mapper = mapper;
            this.Amphorae = new List<Amphora.Common.Models.AmphoraModel>();
        }

        [BindProperty]
        public IEnumerable<Amphora.Common.Models.AmphoraModel> Amphorae { get; set; }

        public async Task<IActionResult> OnGetAsync(string orgId, string geoHash)
        {
            var user = await this.userManager.GetUserAsync(User);

            if(! string.IsNullOrEmpty(geoHash))
            {
                this.Amphorae = await this.amphoraeService.AmphoraStore.StartsWithQueryAsync("GeoHash", geoHash);
                return Page();
            }
            if (orgId != null)
            {
                this.Amphorae = await amphoraeService.AmphoraStore.ListAsync(orgId);
            }
            else
            {
                this.Amphorae = await amphoraeService.AmphoraStore.ListAsync();
            }
            
            return Page();
        }
    }
}