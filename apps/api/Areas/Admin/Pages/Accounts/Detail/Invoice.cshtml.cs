using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Organisations.Accounts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Areas.Admin.Pages.Accounts.Detail
{
    [GlobalAdminAuthorize]
    public class InvoicePageModel : PageModel
    {
        private readonly IOrganisationService organisationService;
        private readonly IInvoiceFileService invoiceFileService;
        private readonly IAccountsService accountsService;
        private readonly ILogger<InvoicePageModel> logger;

        public InvoicePageModel(IOrganisationService organisationService,
                                IInvoiceFileService invoiceFileService,
                                IAccountsService accountsService,
                                ILogger<InvoicePageModel> logger)
        {
            this.organisationService = organisationService;
            this.invoiceFileService = invoiceFileService;
            this.accountsService = accountsService;
            this.logger = logger;
        }

        public OrganisationModel Organisation { get; private set; }
        public List<SelectListItem> ActionOptions { get; set; }
            = new List<SelectListItem>
            {
                new SelectListItem("Preview", "preview"),
                new SelectListItem("Create", "create")
            };

        [BindProperty]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{yyyy-MM}")]
        public DateTimeOffset? Month { get; set; }
        [BindProperty]
        public bool Regenerate { get; set; }
        [BindProperty]
        public string Action { get; set; }
        public Invoice Invoice { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id, string invoiceId)
        {
            this.Organisation = await organisationService.Store.ReadAsync(id);
            if (invoiceId != null)
            {
                this.Invoice = this.Organisation.Account.Invoices.FirstOrDefault(_ => _.Id == invoiceId);
            }

            return Page();
        }

        public async Task<IActionResult> OnGetDownloadCsvAsync(string id, string invoiceId)
        {
            this.Organisation = await organisationService.Store.ReadAsync(id);
            if (invoiceId != null)
            {
                this.Invoice = this.Organisation.Account.Invoices.FirstOrDefault(_ => _.Id == invoiceId);

                var f = await invoiceFileService.GetTransactionsAsCsvFileAsync(Invoice);
                return File(f.Raw, "text/csv", $"invoice-{f.FileName}");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            this.Organisation = await organisationService.Store.ReadAsync(id);
            if (!this.Month.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Month is required");
                return Page();
            }

            switch (Action?.ToLower())
            {
                case "preview":
                    this.Invoice = await accountsService.GenerateInvoiceAsync(Month.Value, id, isPreview: true, regenerate: Regenerate);
                    break;
                case "create":
                    this.Invoice = await accountsService.GenerateInvoiceAsync(Month.Value, id, isPreview: false, regenerate: Regenerate);
                    break;
            }

            return Page();
        }
    }
}