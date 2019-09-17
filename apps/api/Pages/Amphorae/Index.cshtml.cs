using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
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
        private readonly IMapper mapper;

        public IndexModel(
            IUserManager userManager,
            IAmphoraeService amphoraeService,
            IEntityStore<AmphoraModel> entityStore,
            IMapper mapper)
        {
            this.userManager = userManager;
            this.amphoraeService = amphoraeService;
            this.mapper = mapper;
            this.Amphorae = new List<AmphoraModel>();
        }

        [BindProperty]
        public IEnumerable<AmphoraModel> Amphorae { get; set; }

        public async Task<IActionResult> OnGetAsync(string orgId, string geoHash)
        {
            var user = await this.userManager.GetUserAsync(User);

            if(! string.IsNullOrEmpty(geoHash))
            {
                this.Amphorae = await this.amphoraeService.AmphoraStore.StartsWithQueryAsync<AmphoraExtendedModel>("GeoHash", geoHash);
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