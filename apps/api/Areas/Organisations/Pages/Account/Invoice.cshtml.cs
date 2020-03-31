using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Organisations.Pages.Account
{
    public class InvoicePageModel : OrganisationPageModel
    {
        private readonly IInvoiceFileService invoiceFileService;

        public InvoicePageModel(IUserDataService userDataService,
                                IOrganisationService organisationService,
                                IInvoiceFileService invoiceFileService) : base(organisationService, userDataService)
        {
            this.invoiceFileService = invoiceFileService;
        }

        public Invoice Invoice { get; private set; }

        public async Task<IActionResult> OnGetAsync(string invoiceId)
        {
            if (await LoadPropertiesAsync())
            {
                if (IsAdmin)
                {
                    this.Invoice = this.Organisation.Account.Invoices.FirstOrDefault(_ => _.Id == invoiceId);
                    if (this.Invoice == null)
                    {
                        return NotFound();
                    }

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

        public async Task<IActionResult> OnGetDownloadCsvAsync(string invoiceId)
        {
            if (await LoadPropertiesAsync())
            {
                if (IsAdmin)
                {
                    this.Invoice = this.Organisation.Account.Invoices.FirstOrDefault(_ => _.Id == invoiceId);
                    if (this.Invoice == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        var f = await invoiceFileService.GetTransactionsAsCsvFileAsync(Invoice);
                        return File(f.Raw, "text/csv", $"invoice-{f.FileName}");
                    }
                }
                else
                {
                    return StatusCode(403);
                }
            }
            else
            {
                return NotFound();
            }
        }
    }
}