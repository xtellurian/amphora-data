using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Organisations.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages.Accounts
{
    [Authorize]
    public class CreditsModel : PageModel
    {
        private readonly IUserService userService;

        public CreditsModel(IUserService userService)
        {
            this.userService = userService;
        }

        public OrganisationModel Organisation { get; private set; }
        public Account Account { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userService.ReadUserModelAsync(User);
            if(! user.IsAdmin())
            {
                return StatusCode(403);
            }
            this.Organisation = user.Organisation;
            this.Account = this.Organisation.Account;
            return Page();
        }
    }
}