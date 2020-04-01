using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Admin.Pages.Accounts
{
    [GlobalAdminAuthorize]
    public class IndexPageModel : AccountsPageModel
    {
        private readonly IAccountsService accountsService;

        public IndexPageModel(IEntityStore<OrganisationModel> orgStore, IAccountsService accountsService) : base(orgStore)
        {
            this.accountsService = accountsService;
        }

        [TempData]
        public string Message { get; set; } = null;

        public async Task<IActionResult> OnGetAsync(int pageNumber = 0, int perPage = 10, string name = null)
        {
            await LoadOrgsAsync(pageNumber, perPage, name);
            return Page();
        }

        public async Task<IActionResult> OnPostGenerateTransactionsAsync(int pageNumber = 0, int perPage = 10, string name = null)
        {
            await LoadOrgsAsync(pageNumber, perPage, name);
            await accountsService.PopulateDebitsAndCreditsAsync();
            Message = "Done";
            return Page();
        }
    }
}