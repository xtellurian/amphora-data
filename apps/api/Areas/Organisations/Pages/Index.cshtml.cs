using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages
{
    [CommonAuthorize]
    public class IndexModel : PageModel
    {
        private readonly ISearchService searchService;

        [BindProperty(SupportsGet = true)]
        public string Term { get; set; }

        public IndexModel(ISearchService searchService)
        {
            this.searchService = searchService;
        }

        public IList<OrganisationModel> Orgs { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var results = await searchService.SearchAsync<OrganisationModel>(Term);
            Orgs = results.Results.Select(_ => _.Entity).ToList();
            return Page();
        }
    }
}