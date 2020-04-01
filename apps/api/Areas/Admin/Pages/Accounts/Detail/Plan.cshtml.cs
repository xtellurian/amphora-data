using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Admin.Pages.Accounts.Detail
{
    public class PlanPageModel : AccountDetailPageModel
    {
        public PlanPageModel(IEntityStore<OrganisationModel> orgStore) : base(orgStore)
        {
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (await LoadOrganisationAsync(id))
            {
                return Page();
            }
            else
            {
                return BadRequest(Error);
            }
        }
    }
}