using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Organisations.Pages.Account
{
    [CommonAuthorize]
    public class DebitsPageModel : OrganisationPageModel
    {
        public DebitsPageModel(IUserDataService userDataService, IOrganisationService organisationService)
        : base(organisationService, userDataService)
        { }

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
                return NotFound(Error);
            }
        }
    }
}