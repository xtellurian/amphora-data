using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Search;
using Amphora.Common.Models.Amphorae;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Amphorae
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUserService userService;
        private readonly ISearchService searchService;
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
            this.mapper = mapper;
            this.Amphorae = new List<AmphoraModel>();
        }

        [BindProperty]
        public IEnumerable<AmphoraModel> Amphorae { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await this.userService.UserManager.GetUserAsync(User);
            if(user == null)
            {
                return RedirectToPage("/Account/Login", new {area = "Identity"});
            }
            // get my amphora
            var results = await searchService.SearchAmphora("", SearchParameters.ForUserAsCreator(user));
            this.Amphorae = results.Results.Select(s => s.Entity);
            return Page();
        }
    }
}