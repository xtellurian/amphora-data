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
    public class CreditsModel : PageModel
    {
        private readonly IUserService userService;
        private readonly IOrganisationService organisationService;

        public CreditsModel(IUserService userService, IOrganisationService organisationService)
        {
            this.userService = userService;
            this.organisationService = organisationService;
        }

        public OrganisationModel Organisation { get; private set; }
        public Account Account { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userService.ReadUserModelAsync(User);
            var res = await organisationService.ReadAsync(User, user.OrganisationId);
            if (res.Succeeded && !res.Entity.IsAdministrator(user))
            {
                // User must be Admin
                return StatusCode(403);
            }

            this.Organisation = res.Entity;
            this.Account = this.Organisation.Account;
            return Page();
        }
    }
}