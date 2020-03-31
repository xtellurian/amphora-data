using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Areas.Organisations.Pages.Account
{
    [CommonAuthorize]
    public class PlanPageModel : OrganisationPageModel
    {
        private readonly ILogger<PlanPageModel> logger;

        public PlanPageModel(IOrganisationService organisationService,
                              IUserDataService userDataService,
                              ILogger<PlanPageModel> logger)
        : base(organisationService, userDataService)
        {
            this.logger = logger;
        }

        public string PlanName { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (await LoadPropertiesAsync())
            {
                this.PlanName = this.Organisation.Account?.Plan?.PlanType.ToString() ?? "Free";
                return Page();
            }
            else
            {
                // something went wrong
                logger.LogError("Failed to load properties");
                return RedirectToPage("../Detail");
            }
        }
    }
}