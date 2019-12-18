using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Organisations.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages.Accounts
{
    [Authorize]
    public class InvoicesPageModel : PageModel
    {
        private readonly IOrganisationService organisationService;
        private readonly IUserService userService;

        public InvoicesPageModel(IOrganisationService organisationService, IUserService userService)
        {
            this.organisationService = organisationService;
            this.userService = userService;
        }

        public OrganisationModel Organisation { get; private set; }
        public ICollection<Invoice> Invoices { get; private set; }

        public IEnumerable<Invoice> PaidInvoices => GetPaidInvoices();
        public IEnumerable<Invoice> UnpaidInvoices => GetUnpaidInvoices();

        private IEnumerable<Invoice> GetUnpaidInvoices()
        {
            return this.Organisation.Account.Invoices.Where(_ => _.IsPaid.HasValue
                && _.IsPaid.Value
                && _.IsPreview.HasValue
                && _.IsPreview.Value);
        }

        private IEnumerable<Invoice> GetPaidInvoices()
        {
            return this.Organisation.Account.Invoices.Where(_ => _.IsPaid.HasValue
            && _.IsPaid.Value
            && _.IsPreview.HasValue
            && _.IsPreview.Value);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userService.ReadUserModelAsync(User);
            var res = await organisationService.ReadAsync(User, user.OrganisationId);
            if (res.Succeeded)
            {
                this.Organisation = res.Entity;
                this.Invoices = this.Organisation.Account?.Invoices ?? new List<Invoice>();

                return Page();
            }
            else if (res.WasForbidden)
            {
                return StatusCode(403);
            }
            else
            {
                return BadRequest(res.Message);
            }
        }
    }
}