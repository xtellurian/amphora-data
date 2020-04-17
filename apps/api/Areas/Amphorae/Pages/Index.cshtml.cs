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
    public class IndexPageModel : AmphoraeIndexPageModel
    {
        private readonly IUserDataService userDataService;
        private readonly ISearchService searchService;
        private readonly IMapper mapper;

        public IndexPageModel(
            IUserDataService userDataService,
            ISearchService searchService,
            IAmphoraeService amphoraeService,
            IEntityStore<AmphoraModel> entityStore,
            IMapper mapper) : base(amphoraeService, userDataService)
        {
            this.userDataService = userDataService;
            this.searchService = searchService;
            this.mapper = mapper;
        }

        public string Handler { get; private set; }
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
        }
}
