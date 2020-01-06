using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Amphora.Api.Areas.Amphorae.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUserService userService;
        private readonly ISearchService searchService;
        private readonly IAmphoraeService amphoraeService;
        private readonly IMapper mapper;

        public IndexModel(
            IUserService userService,
            ISearchService searchService,
            IAmphoraeService amphoraeService,
            IEntityStore<AmphoraModel> entityStore,
            IMapper mapper)
        {
            this.userService = userService;
            this.searchService = searchService;
            this.amphoraeService = amphoraeService;
            this.mapper = mapper;
            this.Amphorae = new List<AmphoraModel>();
        }

        private const int DefaultTop = 8;
        public IEnumerable<AmphoraModel> Amphorae { get; set; }
        public string ViewType { get; private set; }
        public int? Count { get; set; } = null;
        public int? Skip { get; set; } = 0;
        public int? Top { get; set; } = DefaultTop;

        public int TotalSkip => (Skip ?? 0) * (Top ?? DefaultTop);

        public async Task<IActionResult> OnGetAsync(string viewType, int? skip = 0, int? top = DefaultTop)
        {
            Skip = skip;
            Top = top;
            switch (viewType?.ToLower())
            {
                case "purchased":
                    ViewType = "purchased";
                    return await MyPurchasedAmphorae();
                case "org":
                    ViewType = "org";
                    return await OrgAmphorae();
                case "mine":
                default:
                    ViewType = "mine";
                    return await MyAmphorae();
            }
        }

        private async Task<IActionResult> MyPurchasedAmphorae()
        {
            var user = await this.userService.UserManager.GetUserAsync(User);
            if (user == null) { return RedirectToPage("/Account/Login", new { area = "Profiles" }); }

            var amphorae = await amphoraeService.AmphoraPurchasedBy(User, user);
            Count = await amphorae.CountAsync();
            this.Amphorae = await amphorae.Skip(TotalSkip).Take(Top ?? 0).ToListAsync();
            return Page();
        }

        private async Task<IActionResult> OrgAmphorae()
        {
            var user = await this.userService.ReadUserModelAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Profiles" });
            }

            // get my amphora
            var amphorae = amphoraeService.AmphoraStore.Query(a => a.OrganisationId == user.OrganisationId);
            Count = await amphorae.CountAsync();
            this.Amphorae = await amphorae.Skip(TotalSkip).Take(Top ?? 0).ToListAsync();
            return Page();
        }

        private async Task<IActionResult> MyAmphorae()
        {
            var user = await this.userService.UserManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Profiles" });
            }

            // get my amphora
            var amphorae = amphoraeService.AmphoraStore.Query(a => a.CreatedById == user.Id);
            Count = await amphorae.CountAsync();
            this.Amphorae = await amphorae.Skip(TotalSkip).Take(Top ?? 0).ToListAsync();
            return Page();
        }
    }
}
