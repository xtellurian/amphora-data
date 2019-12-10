using System;
using System.Threading.Tasks;
using Amphora.Common.Models.Organisations.Accounts;

namespace Amphora.Api.Contracts
{
    public interface IAccountsService
    {
        Task<Invoice> GenerateInvoiceAsync(DateTimeOffset month,
                                           string organisationId,
                                           bool isPreview = true,
                                           bool regenerate = false);
        Task PopulateDebitsAsync();
        Task<Invoice> PublishInvoice(Invoice invoice);
    }
}