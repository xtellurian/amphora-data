using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Services.Purchases;
using Amphora.Api.Stores.InMemory;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Organisations.Accounts;
using FluentAssertions;
using Xunit;

namespace Amphora.Tests.Unit.Services
{
    public class InvoiceFileServiceTests : UnitTestBase
    {
        [Fact]
        public async Task CanProduceCsv()
        {
            var orgId = "1234";
            var invoiceId = "5667";
            var dtProvider = GetMockDateTimeProvider();
            var blobStore = new InMemoryBlobStore<OrganisationModel>(dtProvider, CreateMockLogger<InMemoryBlobStore<OrganisationModel>>());

            var sut = new InvoiceFileService(blobStore);

            var invoice = new Invoice
            {
                Id = invoiceId,
                Account = new Account
                {
                    OrganisationId = orgId,
                    Organisation = new OrganisationModel
                    {
                        Id = orgId
                    }
                },
                Transactions = new List<InvoiceTransaction>
                {
                    new InvoiceTransaction("label", 55)
                }
            };

            var wrapper = await sut.GetTransactionsAsCsvFileAsync(invoice);
            wrapper.FileName.Should().NotBeNullOrEmpty();
            wrapper.FileName.Should().Contain(invoiceId);
            wrapper.Raw.Should().NotBeEmpty();
        }
    }
}