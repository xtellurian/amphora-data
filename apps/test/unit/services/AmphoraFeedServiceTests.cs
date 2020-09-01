using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Services.Feeds;
using Amphora.Api.Stores.EFCore;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Purchases;
using Amphora.Common.Models.Users;
using Amphora.Tests.Helpers;
using Amphora.Tests.Mocks;
using FluentAssertions;
using Xunit;

namespace Amphora.Tests.Unit.Services
{
    public class AmphoraFeedServiceTests : UnitTestBase
    {
        private IDateTimeProvider dtProvider = new MockDateTimeProvider();

        private async Task<List<AmphoraModel>> CreateSomeAmphora(AmphoraeEFStore store, OrganisationModel org)
        {
            var res = new List<AmphoraModel>();
            for (var i = 0; i < 10; i++)
            {
                var a = EntityLibrary.GetAmphoraModel(org);
                res.Add(await store.CreateAsync(a));
            }

            return res;
        }

        private async Task PurchaseSomeAmphora(PurchaseEFStore store, ApplicationUserDataModel user, IEnumerable<AmphoraModel> toPurchase)
        {
            var pTime = DateTimeOffset.Now.AddDays(-1);
            foreach (var a in toPurchase)
            {
                await store.CreateAsync(new PurchaseModel(user, a, pTime));
                pTime = pTime.AddHours(1);
            }
        }

        [Fact]
        public async Task AmphoraFeed_ListsRecentlyCreatedAmphora()
        {
            // setup
            var context = GetContext();
            var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
            var amphoraStore = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());
            var purchaseStore = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());

            var sut = new AmphoraFeedService(amphoraStore, purchaseStore);
            var principal = new TestPrincipal();

            var org = await orgStore.CreateAsync(EntityLibrary.GetOrganisationModel());

            await CreateSomeAmphora(amphoraStore, org);

            var posts = await sut.GetPostsAsync(org, 5);
            posts.Should().NotBeNullOrEmpty();
            posts.Should().HaveCountLessOrEqualTo(5);
            posts.Should().BeInDescendingOrder(_ => _.Timestamp);
        }

        [Fact]
        public async Task AmphoraFeed_ListsRecentlyPurchasedAmphora()
        {
            // setup
            var context = GetContext();
            var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
            var amphoraStore = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());
            var purchaseStore = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());

            var sut = new AmphoraFeedService(amphoraStore, purchaseStore);
            var principal = new TestPrincipal();

            var sellingOrg = await orgStore.CreateAsync(EntityLibrary.GetOrganisationModel());
            var buyingOrg = await orgStore.CreateAsync(EntityLibrary.GetOrganisationModel());
            var userName = Guid.NewGuid().ToString();
            var user = new ApplicationUserDataModel()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = userName,
                Organisation = buyingOrg,
                OrganisationId = buyingOrg.Id
            };

            var amphoras = await CreateSomeAmphora(amphoraStore, sellingOrg);
            await PurchaseSomeAmphora(purchaseStore, user, amphoras);

            var posts = await sut.GetPostsAsync(buyingOrg, 5);
            posts.Should().NotBeNullOrEmpty();
            posts.Should().HaveCountLessOrEqualTo(5);
            posts.Should().BeInDescendingOrder(_ => _.Timestamp);
            foreach (var p in posts)
            {
                p.Text.Should().Contain(userName);
            }
        }
    }
}