using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Organisations.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Pages.Shared.Components
{
    public class InvoiceRowViewComponent : ViewComponent
    {
        private readonly IOrganisationService organisationService;

        public InvoiceRowViewComponent(IOrganisationService organisationService)
        {
            this.organisationService = organisationService;
        }

        public InvoiceModel Invoice { get; private set; }

        public async Task<IViewComponentResult> InvokeAsync(string orgId, string invoiceId)
        {
            var org = await organisationService.Store.ReadAsync(orgId);
            this.Invoice = org.Account.Invoices().FirstOrDefault(_ => _.Id == invoiceId);
            return View(this);
        }
    }
}