using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Organisations.Accounts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages.Accounts
{
    public class InvoicePageModel : PageModel
    {
        private readonly IUserDataService userDataService;
        private readonly IOrganisationService organisationService;
        private readonly IInvoiceFileService invoiceFileService;

        public InvoicePageModel(IUserDataService userDataService, IOrganisationService organisationService, IInvoiceFileService invoiceFileService)
        {
            this.userDataService = userDataService;
            this.organisationService = organisationService;
            this.invoiceFileService = invoiceFileService;
        }

        public OrganisationModel Organisation { get; private set; }
        public Invoice Invoice { get; private set; }

        public async Task<IActionResult> OnGetAsync(string invoiceId)
        {
            var userReadRes = await userDataService.ReadAsync(User);
            if (userReadRes.Succeeded)
            {
                var user = userReadRes.Entity;
                var res = await organisationService.ReadAsync(User, user.OrganisationId);
                if (res.Succeeded && res.Entity.IsAdministrator(user))
                {
                    this.Organisation = res.Entity;
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
                return NotFound();
            }
        }

        public async Task<IActionResult> OnGetDownloadCsvAsync(string invoiceId)
        {
            var userReadRes = await userDataService.ReadAsync(User);
            if (userReadRes.Succeeded)
            {
                var res = await organisationService.ReadAsync(User, userReadRes.Entity.OrganisationId);
                if (res.Succeeded && res.Entity.IsAdministrator(userReadRes.Entity))
                {
                    this.Organisation = res.Entity;
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