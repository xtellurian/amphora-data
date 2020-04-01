using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Organisations.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Admin.Pages.Accounts.Detail
{
    [GlobalAdminAuthorize]
    public class AddCreditPageModel : AccountDetailPageModel
    {
        private readonly IAccountsService accountService;
        private readonly IDateTimeProvider dateTimeProvider;

        public AddCreditPageModel(IEntityStore<OrganisationModel> orgStore, IAccountsService accountService, IDateTimeProvider dateTimeProvider)
        : base(orgStore)
        {
            this.accountService = accountService;
            this.dateTimeProvider = dateTimeProvider;
        }

        [TempData]
        public string Message { get; set; }

        [BindProperty]
        [Required]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "2 decimal places")]
        public double Amount { get; set; }
        [BindProperty]
        [Required]
        [MinLength(5)]
        public string Label { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (await LoadOrganisationAsync(id))
            {
                return Page();
            }
            else
            {
                return BadRequest(Error);
            }
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (await LoadOrganisationAsync(id))
            {
                Org.Account ??= new Account();
                if (Amount > 0)
                {
                    this.Org.Account.CreditAccount(Label, Amount, dateTimeProvider.UtcNow);
                    this.Org = await orgStore.UpdateAsync(Org);
                    Message = $"Organisation {Org.Name} was credited {Amount}";
                }
                else
                {
                    Message = "Amount was 0. No action taken.";
                }

                return Page();
            }
            else
            {
                return BadRequest(Error);
            }
        }
    }
}
