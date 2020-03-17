using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Amphora.Api.Areas.Amphorae.Pages
{
    [CommonAuthorize]
    public class IndexModel : PageModel
    {
        private readonly IUserDataService userDataService;
        private readonly ISearchService searchService;
        private readonly IAmphoraeService amphoraeService;
        private readonly IMapper mapper;

        public IndexModel(
            IUserDataService userDataService,
            ISearchService searchService,
            IAmphoraeService amphoraeService,
            IEntityStore<AmphoraModel> entityStore,
            IMapper mapper)
        {
            this.userDataService = userDataService;
            this.searchService = searchService;
            this.amphoraeService = amphoraeService;
            this.mapper = mapper;
            this.Amphorae = new List<AmphoraModel>();
        }

        private const int DefaultTop = 8;
        public IEnumerable<AmphoraModel> Amphorae { get; set; }
        public string Handler { get; private set; }
        public int? Count { get; set; } = null;
        public int? Skip { get; set; } = 0;
        public int? Top { get; set; } = DefaultTop;

        public int TotalSkip => (Skip ?? 0) * (Top ?? DefaultTop);
        [BindProperty]
        public string JsonSelectedAmphoraIdArray { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            return await this.OnGetMineAsync();
        }

        public async Task<IActionResult> OnGetMineAsync(int? skip = 0, int? top = DefaultTop)
        {
            Skip = skip;
            Top = top;
            Handler = "mine";
            return await MyAmphorae();
        }

        public async Task<IActionResult> OnGetOrgAsync(int? skip = 0, int? top = DefaultTop)
        {
            Skip = skip;
            Top = top;
            Handler = "org";
            return await OrgAmphorae();
        }

        public async Task<IActionResult> OnGetPurchasedAsync(int? skip = 0, int? top = DefaultTop)
        {
            Skip = skip;
            Top = top;
            Handler = "purchased";
            return await MyPurchasedAmphorae();
        }

        public ActionResult OnPostDelete()
        {
            List<string> ids = new List<string>();
            try
            {
                ids = JsonConvert.DeserializeObject<List<string>>(JsonSelectedAmphoraIdArray);
            }
            catch (System.Exception)
            {
                return RedirectToPage("./Index");
            }

            if (ids.Count == 0)
            {
                return RedirectToPage("Index");
            }

            // now delete many
            return RedirectToPage("./DeleteMany", new { ids = ids });
        }

        private async Task<IActionResult> MyPurchasedAmphorae()
        {
            var userReadRes = await userDataService.ReadAsync(User);
            if (!userReadRes.Succeeded) { return RedirectToPage("/Account/Login", new { area = "Profiles" }); }

            var amphorae = await amphoraeService.AmphoraPurchasedBy(User, userReadRes.Entity);
            Count = await amphorae.CountAsync();
            this.Amphorae = await amphorae.Skip(TotalSkip).Take(Top ?? 0).ToListAsync();
            return Page();
        }

        private async Task<IActionResult> OrgAmphorae()
        {
            var userReadRes = await userDataService.ReadAsync(User);
            if (!userReadRes.Succeeded)
            {
                return RedirectToPage("/Account/Login", new { area = "Profiles" });
            }

            // get my amphora
            var amphorae = amphoraeService.AmphoraStore.Query(a => a.OrganisationId == userReadRes.Entity.OrganisationId);
            Count = await amphorae.CountAsync();
            this.Amphorae = await amphorae.Skip(TotalSkip).Take(Top ?? 0).ToListAsync();
            return Page();
        }

        private async Task<IActionResult> MyAmphorae()
        {
            var userReadRes = await userDataService.ReadAsync(User);
            if (!userReadRes.Succeeded)
            {
                return RedirectToPage("/Account/Login", new { area = "Profiles" });
            }

            // get my amphora
            var amphorae = amphoraeService.AmphoraStore.Query(a => a.CreatedById == userReadRes.Entity.Id);
            Count = await amphorae.CountAsync();
            this.Amphorae = await amphorae.Skip(TotalSkip).Take(Top ?? 0).ToListAsync();
            return Page();
        }
    }
}
