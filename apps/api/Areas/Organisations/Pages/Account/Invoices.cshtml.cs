using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Organisations.Pages.Account
{
    [CommonAuthorize]
    public class InvoicesPageModel : OrganisationPageModel
    {
        public InvoicesPageModel(IOrganisationService organisationService, IUserDataService userDataService)
        : base(organisationService, userDataService)
        {
        }

        public ICollection<InvoiceModel> Invoices { get; private set; }

        public IEnumerable<InvoiceModel> PaidInvoices => this.Organisation.Account.GetPaidInvoices();
        public IEnumerable<InvoiceModel> UnpaidInvoices => this.Organisation.Account.GetUnpaidInvoices();

        public async Task<IActionResult> OnGetAsync()
        {
            if (await LoadPropertiesAsync())
            {
                if (IsAdmin)
                {
                    this.Invoices = this.Organisation.Account?.Invoices() ?? new List<InvoiceModel>();

                    return Page();
                }
                else
                {
                    return StatusCode(403);
                }
            }
            else
            {
                return NotFound(Error);
            }
        }
    }
}