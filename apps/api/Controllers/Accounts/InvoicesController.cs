using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos;
using Amphora.Api.Models.Dtos.Accounts;
using Amphora.Common.Contracts;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Amphora.Api.Controllers.Accounts
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    [Route("api/account/invoices")]
    [OpenApiTag("Account")]
    public class InvoicesController : EntityController
    {
        private readonly IUserDataService userDataService;
        private readonly IAccountsService accountsService;
        private readonly IOrganisationService orgService;
        private readonly IInvoiceFileService invoiceFileService;
        private readonly IMapper mapper;

        public InvoicesController(IUserDataService userDataService,
                                  IAccountsService accountsService,
                                  IOrganisationService orgService,
                                  IInvoiceFileService invoiceFileService,
                                  IMapper mapper)
        {
            this.userDataService = userDataService;
            this.accountsService = accountsService;
            this.orgService = orgService;
            this.invoiceFileService = invoiceFileService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Creates a new invoice. Restricted to global administrators.
        /// </summary>
        /// <returns>The new invoice.</returns>
        [HttpPost]
        [GlobalAdminAuthorize]
        [Produces(typeof(ItemResponse<Invoice>))]
        [ProducesBadRequest]
        public async Task<IActionResult> CreateInvoice(CreateInvoice createFor)
        {
            var orgReadRes = await orgService.ReadAsync(User, createFor.OrganisationId);
            if (orgReadRes.Succeeded)
            {
                var invoice = await accountsService.GenerateInvoiceAsync(createFor.Month,
                                                                         createFor.OrganisationId,
                                                                         createFor.Preview ?? false,
                                                                         createFor.Regenerate ?? false);

                var dto = mapper.Map<Invoice>(invoice);
                return Ok(new ItemResponse<Invoice>(dto, "Created new invoice"));
            }
            else
            {
                return BadRequest(new Response($"Organisation({createFor.OrganisationId}) not found"));
            }
        }

        /// <summary>
        /// Returns a list of invoices as items.
        /// </summary>
        /// <returns>A list of invoices.</returns>
        [HttpGet]
        [CommonAuthorize]
        [Produces(typeof(CollectionResponse<Invoice>))]
        [ProducesBadRequest]
        public async Task<IActionResult> GetInvoices()
        {
            var userDataRes = await userDataService.ReadAsync(User);
            if (userDataRes.Succeeded)
            {
                var org = userDataRes.Entity.Organisation;
                if (org.IsAdministrator(userDataRes.Entity))
                {
                    var invoices = org.Account.Invoices;
                    var results = mapper.Map<List<Invoice>>(invoices);
                    return Ok(new CollectionResponse<Invoice>(invoices.Count, results));
                }
                else
                {
                    return BadRequest(new Response("You must be an administrator to list Invoices."));
                }
            }
            else
            {
                return BadRequest(new Response("An unknown error occured."));
            }
        }

        /// <summary>
        /// Downloads an invoice in a specified format.
        /// </summary>
        /// <param name="id">Invoice Id.</param>
        /// <param name="format">Only csv is supported.</param>
        /// <returns>The invoice in the format specified.</returns>
        [HttpGet("{id}/download")]
        [CommonAuthorize]
        [Produces(typeof(CollectionResponse<Invoice>))]
        [ProducesBadRequest]
        public async Task<IActionResult> DownloadInvoice(string id, string format)
        {
            if (!string.Equals(format?.ToLower(), "csv"))
            {
                return BadRequest(new Response("format must be csv"));
            }

            var userDataRes = await userDataService.ReadAsync(User);
            if (userDataRes.Succeeded)
            {
                var org = userDataRes.Entity.Organisation;
                if (org.IsAdministrator(userDataRes.Entity))
                {
                    var invoice = org.Account.Invoices.FirstOrDefault(i => i.Id == id);
                    if (invoice == null)
                    {
                        return BadRequest($"Invoice({id}) not found");
                    }

                    var f = await invoiceFileService.GetTransactionsAsCsvFileAsync(invoice);
                    return File(f.Raw, "text/csv; chartset=utf-8", $"invoice-{f.FileName}");
                }
                else
                {
                    return BadRequest(new Response("You must be an administrator to list Invoices."));
                }
            }
            else
            {
                return BadRequest(new Response("An unknown error occured."));
            }
        }
    }
}