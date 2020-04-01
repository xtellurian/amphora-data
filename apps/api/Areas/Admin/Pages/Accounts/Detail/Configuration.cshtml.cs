using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Admin.Pages.Accounts.Detail
{
    [GlobalAdminAuthorize]
    public class ConfigurationPageModel : AccountDetailPageModel
    {
        public ConfigurationPageModel(IEntityStore<OrganisationModel> orgStore) : base(orgStore)
        { }
        [BindProperty]
        public Configuration Configuration { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await LoadOrganisationAsync(id);
            this.Configuration = Org.Configuration ?? new Configuration();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            await LoadOrganisationAsync(id);
            if (this.Configuration == null)
            {
                ModelState.AddModelError(string.Empty, "Configuration was null");
                this.Configuration ??= new Configuration();
                return Page();
            }

            if (this.Configuration.MaximumSignals >= 0)
            {
                // validate it's within reasonable bounds
                Org.Configuration = this.Configuration;
                Configuration = Org.Configuration;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Max Signals Must be >= 0");
            }

            return Page();
        }
    }
}