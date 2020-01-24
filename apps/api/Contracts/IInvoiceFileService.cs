using System.Threading.Tasks;
using Amphora.Api.Models;
using Amphora.Common.Models.Organisations.Accounts;

namespace Amphora.Api.Contracts
{
    public interface IInvoiceFileService
    {
        Task<FileWrapper> GetTransactionsAsCsvFileAsync(Invoice invoice);
    }
}