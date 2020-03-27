using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Amphora.Api.Areas.Admin.Pages.Accounts
{
    [GlobalAdminAuthorize]
    public class IndexPageModel : PageModel
    {
        private readonly IEntityStore<OrganisationModel> orgStore;
        private readonly IAccountsService accountsService;

        public IndexPageModel(IEntityStore<OrganisationModel> orgStore, IAccountsService accountsService)
        {
            this.orgStore = orgStore;
            this.accountsService = accountsService;
        }

        public IList<OrganisationModel> Orgs { get; private set; } = new List<OrganisationModel>();
        public int OrgCount { get; private set; }
        public int PageNumber { get; private set; }
        public int PerPage { get; private set; }
        public int MaxPages { get; private set; }
        public string Name { get; set; }
        [TempData]
        public string Message { get; set; } = null;

        public async Task<IActionResult> OnGetAsync(int pageNumber = 0, int perPage = 10, string name = null)
        {
            OrgCount = await orgStore.CountAsync();
            this.Name = name;
            this.PageNumber = pageNumber;
            this.PerPage = perPage;
            this.MaxPages = 1 + (OrgCount / perPage);
            if (name == null)
            {
                Orgs = await orgStore.Query(_ => true)
                    .Skip(pageNumber * perPage)
                    .Take(perPage)
                    .ToListAsync();
            }
            else
            {
                Orgs = (await orgStore.QueryAsync(_ => true))
                    .Where(_ => _.Name.ToLower().Contains(name))
                    .Skip(pageNumber * perPage)
                    .Take(perPage)
                    .ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostGenerateTransactionsAsync()
        {
            await accountsService.PopulateDebitsAndCreditsAsync();
            Message = "Done";
            return Page();
        }
    }
}