using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Profiles.Pages.Account.Manage
{
    [CommonAuthorize]
    public class IndexModel : PageModel
    {
        private readonly IUserService userService;

        public IndexModel(IUserService userService)
        {
            this.userService = userService;
        }

        public ApplicationUser AppUser { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            this.AppUser = await userService.ReadUserModelAsync(User);
            return Page();
        }
    }
}