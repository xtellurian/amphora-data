using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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

        public async Task<FileWrapper> GetTransactionsAsCsvFileAsync(InvoiceModel invoice)
        {
            // check if the file exists
            var path = GetBlobPath(invoice);
            if (!await blobStore.ExistsAsync(invoice.Organisation, path))
            {
                var stream = new MemoryStream();
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture, true)) // leave open = true
                {
                    csv.Configuration.RegisterClassMap<InvoiceTransactionCsvMap>();
                    csv.WriteHeader<InvoiceTransaction>();
                    csv.NextRecord();
                    await csv.WriteRecordsAsync(invoice.Transactions.OrderBy(_ => _.Timestamp));
                    await writer.FlushAsync();
                    stream.Position = 0;
                    await blobStore.WriteAsync(invoice.Organisation, path, stream);
                }
            }

            var contents = await blobStore.ReadBytesAsync(invoice.Organisation, path);
            return new FileWrapper(contents, $"{invoice.Id}{CsvExtension}");
        }

        private string GetBlobPath(InvoiceModel invoice)
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