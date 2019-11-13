using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Organisations.Accounts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Admin.Pages.Accounts
{
    [GlobalAdminAuthorize]
    public class AddCreditPageModel : PageModel
    {
        private readonly IEntityStore<OrganisationModel> orgStore;
        private readonly IAccountsService accountService;

        public AddCreditPageModel(IEntityStore<OrganisationModel> orgStore, IAccountsService accountService)
        {
            this.orgStore = orgStore;
            this.accountService = accountService;
        }

        public string Id { get; private set; }
        public OrganisationModel Organisation { get; private set; }
        [TempData]
        public string Message { get; set; }

        [BindProperty]
        public double Amount { get; set; }
        [BindProperty]
        public string Label { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if(id == null) return RedirectToPage("./Index");
            this.Id = id;
            this.Organisation = await orgStore.ReadAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if(id == null) return RedirectToPage("./Index");
            this.Id = id;
            this.Organisation = await orgStore.ReadAsync(id);
            this.Organisation.Account ??= new Account();
            if(Amount > 0)
            {
                this.Organisation.Account.CreditAccount(Label, Amount);
                this.Organisation = await orgStore.UpdateAsync(Organisation);
                Message = $"Organisation {Organisation.Name} was credited {Amount}";
            }
            else
            {
                Message = "Amount was 0. No action taken.";
            }
            return Page();
        }
    }
}