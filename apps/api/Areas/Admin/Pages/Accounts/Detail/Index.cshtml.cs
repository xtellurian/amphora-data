using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Admin.Pages.Accounts.Detail
{
    [GlobalAdminAuthorize]
    public class IndexPageModel : AccountDetailPageModel
    {
        public IndexPageModel(IEntityStore<OrganisationModel> orgStore) : base(orgStore)
        { }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await LoadOrganisationAsync(id);
            return Page();
        }
    }
}