using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api.Models.Dtos;
using Amphora.Api.Models.Dtos.Accounts;
using Amphora.Api.Models.Dtos.Accounts.Memberships;
using Amphora.Api.Models.Dtos.Amphorae;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Amphora.Tests.Integration.Accounts
{
    [Collection(nameof(ApiFixtureCollection))]
    public class TransactionsTests : WebAppIntegrationTestBase
    {
        public TransactionsTests(WebApplicationFactory<Amphora.Api.Startup> factory) : base(factory)
        { }

        [Fact]
        public async Task GetTransactions_AsStandardUser()
        {
            // make a purchase to ensure at least 1 transaction
            var seller = await GetPersonaAsync(Personas.AmphoraAdmin);
            var buyer = await GetPersonaAsync(Personas.StandardTwo);

            var amphora = Helpers.EntityLibrary.GetAmphoraDto(seller.Organisation.Id);
            var createResponse = await seller.Http.PostAsJsonAsync("api/amphorae", amphora);
            amphora = await AssertHttpSuccess<DetailedAmphora>(createResponse);

            // buyer purchase
            var purchaseResponse = await buyer.Http.PostAsJsonAsync($"api/Amphorae/{amphora.Id}/Purchases", new { });
            await AssertHttpSuccess(purchaseResponse);

            // Arrange
            var persona = await GetPersonaAsync(Personas.Standard);

            // get an Amphora to purchase
            var transactionsReadRes = await persona.Http.GetAsync("api/account/transactions");
            var transactions = await AssertHttpSuccess<CollectionResponse<Transaction>>(transactionsReadRes);

            transactions.Count.Should().NotBeNull().And.BeGreaterThan(0);
            transactions.Items.Should().NotBeEmpty();
            transactions.Items.Should().HaveCountGreaterOrEqualTo(1);
            transactions.Items.Should().Contain(m => m.AmphoraId == amphora.Id);
        }
    }
}