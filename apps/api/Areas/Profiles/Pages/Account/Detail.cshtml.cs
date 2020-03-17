using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Amphora.Api.Areas.Profiles.Pages.Account
{
    [CommonAuthorize]
    public class DetailModel : PageModel
    {
        private readonly IUserDataService userDataService;
        private readonly IAmphoraeService amphoraeService;

        public DetailModel(IUserDataService userDataService, IAmphoraeService amphoraeService)
        {
            this.userDataService = userDataService;
            this.amphoraeService = amphoraeService;
        }

        public ApplicationUserDataModel AppUser { get; set; }

        public bool IsSelf { get; set; }
        public IEnumerable<AmphoraModel> PinnedAmphorae { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id, string userName)
        {
            var readUserRes = await userDataService.ReadAsync(User);
            if (!string.IsNullOrEmpty(id))
            {
                var readRes = await userDataService.ReadAsync(User, id);
                if (readRes.Succeeded)
                {
                    this.AppUser = readRes.Entity;
                }
            }
            else if (!string.IsNullOrEmpty(userName))
            {
                var users = userDataService.Query(User, _ => _.UserName == userName);
                AppUser = await users.FirstOrDefaultAsync();
            }
            else
            {
                AppUser = readUserRes.Entity;
            }

            if (AppUser == null)
            {
                return RedirectToPage("./UserMissing");
            }

            if (AppUser.Id == readUserRes.Entity.Id)
            {
                IsSelf = true;
            }

            var q = amphoraeService.AmphoraStore.Query(a => a.CreatedById == AppUser.Id);
            this.PinnedAmphorae = AppUser.PinnedAmphorae.AreAllNull()
                ? await q.Take(6).ToListAsync()
                : AppUser.PinnedAmphorae as IEnumerable<AmphoraModel>;
            return Page();
        }
    }
}