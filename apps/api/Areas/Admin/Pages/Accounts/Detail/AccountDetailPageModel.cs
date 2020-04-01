using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Dtos;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Admin.Pages.Accounts
{
    public abstract class AccountDetailPageModel : PageModel
    {
        protected readonly IEntityStore<OrganisationModel> orgStore;
        protected Error Error { get; set; }

        protected AccountDetailPageModel(IEntityStore<OrganisationModel> orgStore)
        {
            this.orgStore = orgStore;
        }

        public OrganisationModel Org { get; protected set; }

        protected async Task<bool> LoadOrganisationAsync(string id)
        {
            if (id == null)
            {
                this.Error = new Error("id cannot be null");
                return false;
            }

            this.Org = await orgStore.ReadAsync(id);
            if (Org is null)
            {
                Error = new Error($"Unknown org {id}");
                return false;
            }

            return true;
        }
    }
}