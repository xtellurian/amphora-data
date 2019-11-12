using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Users;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Amphora.Api.Pages.Profile
{
    [Authorize]
    public class DetailModel : PageModel
    {
        private readonly IUserService userService;
        private readonly IAmphoraeService amphoraeService;

        public DetailModel(IUserService userService, IAmphoraeService amphoraeService)
        {
            this.userService = userService;
            this.amphoraeService = amphoraeService;
        }

        public ApplicationUser AppUser { get; set; }

        public bool IsSelf { get; set; }
        public IEnumerable<AmphoraModel> PinnedAmphorae { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id, string userName)
        {
            var currentUser = await userService.ReadUserModelAsync(User);
            if (!string.IsNullOrEmpty(id))
            {
                AppUser = await userService.UserManager.FindByIdAsync(id);
            }
            else if (!string.IsNullOrEmpty(userName))
            {
                AppUser = await userService.UserManager.FindByNameAsync(userName);
            }
            else
            {
                AppUser = currentUser;
            }

            if (AppUser == null)
            {
                return RedirectToPage("./Missing");
            }

            if (AppUser.Id == currentUser.Id)
            {
                IsSelf = true;
            }

            var q = amphoraeService.AmphoraStore.Query(a => a.CreatedById == AppUser.Id);
            this.PinnedAmphorae = await q.Take(6).ToListAsync();
            return Page();
        }

    }
}