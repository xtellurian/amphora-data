using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages.Restrictions
{
    [CommonAuthorize]
    public class IndexPageModel : AmphoraPageModel
    {
        private readonly IRestrictionService restrictionService;

        public IndexPageModel(IAmphoraeService amphoraeService, IRestrictionService restrictionService) : base(amphoraeService)
        {
            this.restrictionService = restrictionService;
        }

        public List<RestrictionModel> Restrictions { get; private set; } = new List<RestrictionModel>();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await LoadAmphoraAsync(id);
            if (Result.Succeeded)
            {
                var amphoraRes = await restrictionService.Store.QueryAsync(_ => _.SourceAmphoraId == Amphora.Id);
                var inheritedRes = await restrictionService.Store.QueryAsync(_ =>
                    _.SourceOrganisationId == Amphora.OrganisationId && _.Scope == RestrictionScope.Organisation);

                this.Restrictions.AddRange(amphoraRes);
                this.Restrictions.AddRange(inheritedRes);
            }

            return OnReturnPage();
        }
    }
}