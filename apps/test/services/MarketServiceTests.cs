using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Users;
using Amphora.Api.Services.Amphorae;
using Amphora.Api.Services.Auth;
using Amphora.Api.Services.Basic;
using Amphora.Api.Services.Market;
using Amphora.Api.Stores.EFCore;
using Amphora.Tests.Helpers;
using Moq;
using Xunit;
using Microsoft.Extensions.Options;

namespace Amphora.Tests.Unit
{
    public class MarketServiceTests : UnitTestBase
    {

        public MarketServiceTests()
        {
        }

        [Fact]
        public async Task LookupByGeo()
        {
            using (var context = GetContext(nameof(MarketServiceTests)))
            {
                var amphoraStore = new AmphoraeEFStore(context, CreateMockLogger<AmphoraeEFStore>());
                var purchaseStore = new PurchaseEFStore(context, CreateMockLogger<PurchaseEFStore>());
                var orgStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
                var mockUserService = new Mock<IUserService>();
                mockUserService.Setup(o => o.ReadUserModelAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser());

                var permissionService = new PermissionService(orgStore, amphoraStore, CreateMockLogger<PermissionService>());
                var options = Mock.Of<IOptionsMonitor<Api.Options.AmphoraManagementOptions>>(_ => _.CurrentValue == new Api.Options.AmphoraManagementOptions());
                var amphoraService = new AmphoraeService(options,
                                                         amphoraStore,
                                                         purchaseStore,
                                                         orgStore,
                                                         permissionService,
                                                         mockUserService.Object,
                                                         CreateMockLogger<AmphoraeService>());
                var service = new BasicSearchService(amphoraService);
                var orgModel = EntityLibrary.GetOrganisationModel();
                var amphora = EntityLibrary.GetAmphoraModel(orgModel, nameof(MarketServiceTests)); // dumy org id
                var entity = await amphoraStore.CreateAsync(amphora);
                var sut = new MarketService(service, amphoraService, Mapper, mockUserService.Object, CreateMemoryCache()) as IMarketService;

                var response = await sut.FindAsync("");
                Assert.NotNull(response);
                Assert.NotEmpty(response);
                Assert.Contains(response, e => e.Id == entity.Id);
            }
        }
    }
}