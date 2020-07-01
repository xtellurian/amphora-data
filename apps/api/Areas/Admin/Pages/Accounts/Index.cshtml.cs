using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Admin;
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

        [BindProperty]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{yyyy-MM}")]
        public DateTimeOffset? Month { get; set; }

        public Report Report { get; private set; }
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
            this.Report = await accountsService.PopulateDebitsAndCreditsAsync(this.Month);
            Message = "Done";
            return Page();
        }
    }
}