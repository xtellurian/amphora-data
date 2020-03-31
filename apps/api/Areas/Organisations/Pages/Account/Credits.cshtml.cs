using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Areas.Organisations.Pages.Account
{
    [CommonAuthorize]
    public class CreditsPageModel : OrganisationPageModel
    {
        private readonly ILogger<CreditsPageModel> logger;

        public CreditsPageModel(IUserDataService userDataService,
                            IOrganisationService organisationService,
                            ILogger<CreditsPageModel> logger)
        : base(organisationService, userDataService)
        {
            this.logger = logger;
        }

        public Amphora.Common.Models.Organisations.Accounts.Account Account { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (await LoadPropertiesAsync())
            {
                this.Account = Organisation.Account;
                if (!IsAdmin)
                {
                    return StatusCode(403);
                }

                return Page();
            }
            else
            {
                return NotFound();
            }
        }
    }
}