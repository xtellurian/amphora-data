using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos;
using Amphora.Api.Models.Dtos.Accounts;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Tests.Helpers;
using Amphora.Tests.Mocks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Amphora.Tests.Integration.Accounts
{
    [Collection(nameof(ApiFixtureCollection))]
    public class InvoiceTests : WebAppIntegrationTestBase
    {
        private static MockDateTimeProvider dtProvider = new MockDateTimeProvider();
        public InvoiceTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory, s => s.AddSingleton<IDateTimeProvider>(dtProvider))
        { }

        private async Task GenerateSomeInvoices(Persona p)
        {
            var admin = await GetPersonaAsync(Personas.AmphoraAdmin);
            var createInvoiceRes = await admin.Http.PostAsJsonAsync("api/invoices",
                new CreateInvoice(DateTimeOffset.Now.AddMonths(-1), p.Organisation.Id, false, true));

            var res = await AssertHttpSuccess<ItemResponse<Invoice>>(createInvoiceRes);
            var invoice = res.Item;
            invoice.Should().NotBeNull();
        }

        private async Task GenerateSomeTransactions(Persona p)
        {
            var other = await GetPersonaAsync(Personas.Other);
            dtProvider.GetNow = () => DateTimeOffset.Now.AddMonths(-1);
            var amphora = EntityLibrary.GetAmphoraDto(other.Organisation.Id);
            var amphoraCreate = await other.Http.PostAsJsonAsync("api/amphorae", amphora);
            amphora = await AssertHttpSuccess<DetailedAmphora>(amphoraCreate);

            var purchaseRes = await p.Http.PostAsJsonAsync($"api/Amphorae/{amphora.Id}/Purchases", new { });
            await AssertHttpSuccess(purchaseRes);
            dtProvider.Reset();
        }

        [Fact]
        public async Task ListInvoices_AsStandardUser()
        {
            // Arrange
            var persona = await GetPersonaAsync(Personas.Standard);
            await GenerateSomeTransactions(persona);
            await GenerateSomeInvoices(persona);
            // Act
            var res = await persona.Http.GetAsync("api/invoices");
            // Assert
            var data = await AssertHttpSuccess<CollectionResponse<Invoice>>(res);
            data.Items.Should().NotBeNull();
            data.Items.Should().NotBeEmpty();
            data.Items.Where(i => i.DateCreated == null).Should().BeEmpty();
            // check there's only one for last month
            var lastMonth = DateTimeOffset.Now.AddMonths(-1).StartOfMonth();
            var thisMonth = lastMonth.AddMonths(1);
            data.Items.Where(i => i.ForMonth >= lastMonth && i.ForMonth <= thisMonth)
                .Should().HaveCount(1, "because we should have only 1 invoice per month");
        }
    }
}