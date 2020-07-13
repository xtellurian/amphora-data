using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Admin.Pages.TermsOfUse
{
    [GlobalAdminAuthorize]
    public class IndexPageModel : PageModel
    {
        private readonly IEntityStore<TermsOfUseModel> store;

        public IndexPageModel(IEntityStore<TermsOfUseModel> store)
        {
            this.store = store;
        }

        public IEnumerable<TermsOfUseModel> Terms { get; private set; } = new List<TermsOfUseModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            this.Terms = await store.QueryAsync(_ => true, 0, 25);
            return Page();
        }
    }
}