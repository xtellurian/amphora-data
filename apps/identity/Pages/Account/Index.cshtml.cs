using System.Threading.Tasks;
using Amphora.Identity.Contracts;
using Amphora.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Identity.Pages.Account
{
    [Authorize]
    public class IndexPageModel : PageModelBase
    {
        private readonly IUserService userService;

        public IndexPageModel(IUserService userService)
        {
            this.userService = userService;
        }

        public ApplicationUser? ApplicationUser { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            this.ApplicationUser = await userService.ReadUserModelAsync(User);
            return Page();
        }
    }
}