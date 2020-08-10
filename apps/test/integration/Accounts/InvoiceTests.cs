using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api.EntityFramework;
using Amphora.Api.Models.Dtos;
using Amphora.Api.Models.Dtos.Accounts;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Purchases;
using Amphora.Tests.Helpers;
using Amphora.Tests.Mocks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Amphora.Tests.Integration.Accounts
{
    [Collection(nameof(ApiFixtureCollection))]
    [TestCaseOrderer("Amphora.Tests.AlphabeticalOrderer", "test")]
    public class InvoiceTests : WebAppIntegrationTestBase
    {
        private static MockDateTimeProvider dtProvider = new MockDateTimeProvider();
        public InvoiceTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        { }

        private async Task GenerateSomeInvoices(Persona p, bool regenerate = false)
        {
            var admin = await GetPersonaAsync(Personas.AmphoraAdmin);
            var ids = new List<string>();
            for (var monthsAgo = 2; monthsAgo >= 0; monthsAgo--)
            {
                var createInvoiceRes = await admin.Http.PostAsJsonAsync("api/account/invoices",
                    new CreateInvoice(DateTimeOffset.Now.AddMonths(-monthsAgo), p.Organisation.Id, false, regenerate));

                var res = await AssertHttpSuccess<ItemResponse<Invoice>>(createInvoiceRes);
                var invoice = res.Item;
                ids.Should().NotContain(invoice.Id, "because each id should be unique");
                invoice.Should().NotBeNull();
                ids.Add(invoice.Id);
            }
        }

        private async Task GenerateSomeTransactions(Persona p)
        {
            var other = await GetPersonaAsync(Personas.Other);
            dtProvider.SetFixed(DateTimeOffset.Now.AddMonths(-1));
            var amphora = EntityLibrary.GetAmphoraDto(other.Organisation.Id);
            var amphoraCreate = await other.Http.PostAsJsonAsync("api/amphorae", amphora);
            amphora = await AssertHttpSuccess<DetailedAmphora>(amphoraCreate);

            using (var scope = CreateScope())
            {
                var context = scope.ServiceProvider.GetService<AmphoraContext>();

                var userData = await context.UserData.FindAsync(p.User.Id);
                var amphoraModel = await context.Amphorae.FindAsync(amphora.Id);
                var purchase = new PurchaseModel(userData, amphoraModel, dtProvider.Now);
                context.Purchases.Add(purchase);
                // create a debit on their account (not mirrored by a credit)
                var org = await context.Organisations.FindAsync(p.Organisation.Id);
                org.Account.DebitAccountFromPurchase(purchase, purchase.CreatedDate);
                await context.SaveChangesAsync();
            }

            dtProvider.Reset();
        }

        [Fact]
        public async Task ListInvoices_AsStandardUser()
        {
            // Arrange
            var persona = await GetPersonaAsync(Personas.Standard);
            await GenerateSomeTransactions(persona);
            await GenerateSomeInvoices(persona, true);
            // Act
            var res = await persona.Http.GetAsync("api/account/invoices");
            // Assert
            var data = await AssertHttpSuccess<CollectionResponse<Invoice>>(res);
            data.Items.Should().NotBeNull();
            data.Items.Should().NotBeEmpty();
            data.Items.Where(i => i.DateCreated == null).Should().BeEmpty();
            // check there's only one for last month
            var lastMonth = DateTimeOffset.Now.AddMonths(-1).StartOfMonth();
            var thisMonth = lastMonth.AddMonths(1);
            data.Items.Where(i => i.Timestamp >= lastMonth && i.Timestamp <= thisMonth)
                .Should().HaveCount(1, "because we should have only 1 invoice per month");
            var invoice = data.Items.FirstOrDefault(i => i.Timestamp >= lastMonth && i.Timestamp <= thisMonth);
            invoice.Transactions.Should().NotBeNullOrEmpty("because we added transactions to last month");
        }

        [Fact]
        public async Task GetInvoice_DownloadCsv()
        {
            // Arrange
            var persona = await GetPersonaAsync(Personas.Standard);
            await GenerateSomeTransactions(persona);
            await GenerateSomeInvoices(persona, false);
            var res = await persona.Http.GetAsync("api/account/invoices");
            var data = await AssertHttpSuccess<CollectionResponse<Invoice>>(res);
            data.Items.Should().NotBeNullOrEmpty();
            var invoice = data.Items.FirstOrDefault(_ => _.Transactions.Any());
            invoice.Should().NotBeNull("because we wanted to select an invoice with transactions");
            invoice.Transactions.Should().NotBeEmpty();
            // Act
            // download csv
            var csvResponse = await persona.Http.GetAsync($"api/account/invoices/{invoice.Id}/download?format=csv");
            await AssertHttpSuccess(csvResponse);
            csvResponse.Content.Headers.ContentType.ToString().Should().Be("text/csv; chartset=utf-8");
            // Assert
            var content = await csvResponse.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty();
        }
    }
}