using Amphora.Common.Models.Organisations.Accounts;
using CsvHelper.Configuration;

namespace Amphora.Api.Services.Purchases
{
    public class InvoiceTransactionCsvMap : ClassMap<InvoiceTransaction>
    {
        public InvoiceTransactionCsvMap()
        {
            Map(_ => _.Timestamp).Name("Timestamp");
            Map(_ => _.RelativeAmount).Name("Amount");
            Map(_ => _.Id).Name("Transaction Id");
            Map(_ => _.AmphoraId).Name("Amphora Id");
            Map(_ => _.Label).Name("Description");
            Map(_ => _.Balance).Name("Balance");
        }
    }
}