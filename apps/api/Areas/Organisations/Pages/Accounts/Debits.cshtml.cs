using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Organisations.Accounts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages.Accounts
{
    [CommonAuthorize]
    public class DebitsModel : PageModel
    {
        private readonly IUserDataService userDataService;
        private readonly IOrganisationService organisationService;

        public DebitsModel(IUserDataService userDataService, IOrganisationService organisationService)
        {
            this.userDataService = userDataService;
            this.organisationService = organisationService;
        }

        public OrganisationModel Organisation { get; private set; }
        public Account Account { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userReadRes = await userDataService.ReadAsync(User);
            if (userReadRes.Succeeded)
            {
                var user = userReadRes.Entity;
                var res = await organisationService.ReadAsync(User, user.Id);

                if (res.Succeeded && !res.Entity.IsAdministrator(user))
                {
                    // must be admin
                    return StatusCode(403);
                }

                this.Organisation = res.Entity;
                this.Account = this.Organisation.Account;
                return Page();
            }
            else
            {
                return NotFound();
            }
        }
    }
}