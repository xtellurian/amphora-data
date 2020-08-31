using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Organisations.Accounts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Amphora.Api.Areas.Admin.Pages.Invoicing
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

        public List<InvoiceModel> Invoices { get; private set; }
            = new List<InvoiceModel>();

        public async Task<IActionResult> OnGetAsync(int page = 0)
        {
            var orgs = orgStore.Query(_ => true);
            foreach (var o in orgs)
            {
                var invoice = o.Invoices.OrderByDescending(_ => _.Timestamp).FirstOrDefault();
                if (invoice != null)
                {
                    Invoices.Add(invoice);
                }
            }

            await Task.Delay(1); // for the asynciness
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.Month.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Month is required");
                return Page();
            }

            Invoices.Clear();
            var orgs = orgStore.Query(_ => true);
            foreach (var o in orgs)
            {
                await GenerateForOrg(o);
            }

            return Page();
        }

        private async Task GenerateForOrg(OrganisationModel org)
        {
            switch (this.Action?.ToLower())
            {
                case "preview":
                    var invoicePreview = await accountsService.GenerateInvoiceAsync(Month.Value, org.Id, isPreview: true, regenerate: Regenerate);
                    Invoices.Add(invoicePreview);
                    break;
                case "create":
                    var invoice = await accountsService.GenerateInvoiceAsync(Month.Value, org.Id, isPreview: false, regenerate: Regenerate);
                    Invoices.Add(invoice);
                    break;
            }
        }
    }
}