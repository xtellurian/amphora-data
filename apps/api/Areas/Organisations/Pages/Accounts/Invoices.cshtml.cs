using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Organisations.Accounts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Areas.Organisations.Pages.Accounts
{
    [CommonAuthorize]
    public class InvoicesPageModel : PageModel
    {
        private readonly IOrganisationService organisationService;
        private readonly IUserDataService userDataService;

        public InvoicesPageModel(IOrganisationService organisationService, IUserDataService userDataService)
        {
            this.organisationService = organisationService;
            this.userDataService = userDataService;
        }

        public OrganisationModel Organisation { get; private set; }
        public ICollection<Invoice> Invoices { get; private set; }

        public IEnumerable<Invoice> PaidInvoices => this.Organisation.Account.GetPaidInvoices();
        public IEnumerable<Invoice> UnpaidInvoices => this.Organisation.Account.GetUnpaidInvoices();

        public async Task<IActionResult> OnGetAsync()
        {
            var userReadRes = await userDataService.ReadAsync(User);
            if (userReadRes.Succeeded)
            {
                var res = await organisationService.ReadAsync(User, userReadRes.Entity.OrganisationId);
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
            else
            {
                return NotFound();
            }
        }
    }
}