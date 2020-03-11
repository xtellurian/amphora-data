using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Admin.Pages.Accounts
{
    [GlobalAdminAuthorize]
    public class ConfigurationPageModel : PageModel
    {
        private readonly IEntityStore<OrganisationModel> orgStore;

        public ConfigurationPageModel(IEntityStore<OrganisationModel> orgStore)
        {
            this.orgStore = orgStore;
        }

        public OrganisationModel Organisation { get; private set; }
        [BindProperty]
        public Configuration Configuration { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await LoadOrgAsync(id);
            this.Configuration = Organisation.Configuration ?? new Configuration();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            await LoadOrgAsync(id);
            if (this.Configuration == null)
            {
                ModelState.AddModelError(string.Empty, "Configuration was null");
                this.Configuration = Organisation.Configuration ?? new Configuration();
                return Page();
            }

            if (this.Configuration.MaximumSignals >= 0)
            {
                // validate it's within reasonable bounds
                Organisation.Configuration = this.Configuration;
                Organisation = await orgStore.UpdateAsync(Organisation);
                Configuration = Organisation.Configuration;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Max Signals Must be >= 0");
            }

            return Page();
        }

        private async Task LoadOrgAsync(string id)
        {
            this.Organisation = await orgStore.ReadAsync(id);
            if (Organisation == null)
            {
                this.ModelState.AddModelError(string.Empty, "Org not found");
            }
        }
    }
}