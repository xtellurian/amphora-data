using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Admin.Pages
{
    [GlobalAdminAuthorize]
    public class PurchasesModel : PageModel
    {
        private readonly IAccountsService accountsService;

        public PurchasesModel(IAccountsService accountsService)
        {
            this.accountsService = accountsService;
        }

        [TempData]
        public string Message { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            await accountsService.PopulateDebitsAndCreditsAsync();
            Message = "Done";
            return Page();
        }
    }
}