using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Organisations.Accounts;
using CsvHelper;

namespace Amphora.Api.Services.Purchases
{
    public class InvoiceFileService : IInvoiceFileService
    {
        private const string InvoicesFolder = "invoices/transactions/";
        private const string CsvExtension = ".csv";
        private readonly IBlobStore<OrganisationModel> blobStore;

        public InvoiceFileService(IBlobStore<OrganisationModel> blobStore)
        {
            this.blobStore = blobStore;
        }

        public async Task<FileWrapper> GetTransactionsAsCsvFileAsync(Invoice invoice)
        {
            // check if the file exists
            var path = GetBlobPath(invoice);
            if (!await blobStore.ExistsAsync(invoice.Account.Organisation, path))
            {
                var stream = await blobStore.GetWritableStreamAsync(invoice.Account.Organisation, path);
                await GenerateFileAsync(invoice, stream);
            }

            var contents = await blobStore.ReadBytesAsync(invoice.Account.Organisation, path);
            return new FileWrapper(contents, $"{invoice.Id}{CsvExtension}");
        }

        private async Task GenerateFileAsync(Invoice invoice, Stream writableStream)
        {
            using (var writer = new StreamWriter(writableStream))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Configuration.RegisterClassMap<InvoiceTransactionCsvMap>();
                csv.WriteHeader<InvoiceTransaction>();
                csv.NextRecord();
                await csv.WriteRecordsAsync(invoice.Transactions.OrderBy(_ => _.Timestamp));
            }
        }

        private string GetBlobPath(Invoice invoice)
        {
            if (invoice.IsPreview == true)
            {
                return $"{InvoicesFolder}{invoice.Id}(preview){CsvExtension}";
            }
            else
            {
                return $"{InvoicesFolder}{invoice.Id}{CsvExtension}";
            }
        }
    }
}