using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Permissions;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages.Restrictions
{
    [CommonAuthorize]
    public class CreatePageModel : AmphoraPageModel
    {
        private readonly IRestrictionService restrictionService;
        private readonly IEntityStore<OrganisationModel> orgStore;

        [BindProperty]
        public Restriction NewRestriction { get; set; }

        public CreatePageModel(IAmphoraeService amphoraeService,
                               IRestrictionService restrictionService,
                               IEntityStore<OrganisationModel> orgStore) : base(amphoraeService)
        {
            this.restrictionService = restrictionService;
            this.orgStore = orgStore;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await LoadAmphoraAsync(id);

            return OnReturnPage();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            await LoadAmphoraAsync(id);

            if (Result.Succeeded)
            {
                if (ModelState.IsValid)
                {
                    var target = await orgStore.ReadAsync(NewRestriction.TargetOrganisationId);
                    if (target == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unknown Organisation");
                        return OnReturnPage();
                    }
                    else
                    {
                        var model = new RestrictionModel(Amphora, target, RestrictionKind.Deny);
                        var res = await restrictionService.CreateAsync(User, model);
                        if (res.Succeeded)
                        {
                            return RedirectToPage("./Index", new { id = Amphora.Id });
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, res.Message);
                            return OnReturnPage();
                        }
                    }
                }
                else
                {
                    return OnReturnPage();
                }
            }

            return OnReturnPage();
        }
    }
}