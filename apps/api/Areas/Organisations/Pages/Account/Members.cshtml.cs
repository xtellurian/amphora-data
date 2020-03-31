using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Areas.Organisations.Pages.Account
{
    [CommonAuthorize]
    public class MembersPageModel : OrganisationPageModel
    {
        private readonly ILogger<MembersPageModel> logger;

        public MembersPageModel(IOrganisationService organisationService,
                                IUserDataService userDataService,
                                ILogger<MembersPageModel> logger)
        : base(organisationService, userDataService)
        {
            this.logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (await LoadPropertiesAsync())
            {
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