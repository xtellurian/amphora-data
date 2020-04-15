using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Platform;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Admin.Pages.Accounts.Detail
{
    [GlobalAdminAuthorize]
    public class LimitsPageModel : AccountDetailPageModel
    {
        private readonly IPlanLimitService planLimitService;
        private readonly IEntityStore<AmphoraModel> amphoraStore;
        private readonly IAmphoraBlobStore blobStore;

        public LimitsPageModel(IEntityStore<OrganisationModel> orgStore,
                               IPlanLimitService planLimitService,
                               IEntityStore<AmphoraModel> amphoraStore,
                               IAmphoraBlobStore blobStore) : base(orgStore)
        {
            this.planLimitService = planLimitService;
            this.amphoraStore = amphoraStore;
            this.blobStore = blobStore;
        }

        public PlanLimits Limits { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (await LoadOrganisationAsync(id))
            {
                this.Limits = await planLimitService.GetLimits(Org);
                return Page();
            }
            else
            {
                return BadRequest(Error);
            }
        }

        public async Task<IActionResult> OnPostCalculateFileSizeAsync(string id)
        {
            if (await LoadOrganisationAsync(id))
            {
                this.Limits = await planLimitService.GetLimits(Org);

                long totalSize = 0;
                var orgsAmphora = amphoraStore.Query(a => a.OrganisationId == Org.Id);
                foreach (var a in orgsAmphora)
                {
                    totalSize += await blobStore.GetContainerSizeAsync(a);
                }

                Org.Cache ??= new DataCache();
                Org.Cache.TotalAmphoraeFileSize = new DataCache.CachedValue<long>(totalSize);
                Org = await orgStore.UpdateAsync(Org);

                return Page();
            }
            else
            {
                return BadRequest(Error);
            }
        }
    }
}