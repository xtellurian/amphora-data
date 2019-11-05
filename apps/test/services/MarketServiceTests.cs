using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Users;
using Amphora.Api.Services.Amphorae;
using Amphora.Api.Services.Auth;
using Amphora.Api.Services.Basic;
using Amphora.Api.Services.Market;
using Amphora.Api.Stores.EFCore;
using Amphora.Common.Models.Organisations;
using Amphora.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Microsoft.Extensions.Options;

namespace Amphora.Tests.Unit
{
    public class MarketServiceTests : UnitTestBase
    {
        private readonly ILogger<PermissionService> permissionLogger;
        private readonly ILogger<AmphoraeService> amphoraLogger;

        public MarketServiceTests(ILogger<PermissionService> permissionLogger, ILogger<AmphoraeService> amphoraLogger)
        {
            this.permissionLogger = permissionLogger;
            this.amphoraLogger = amphoraLogger;
        }

        [Fact]
        public async Task LookupByGeo()
        {
            using (var context = GetContext(nameof(MarketServiceTests)))
            {
                var amphoraStore = new AmphoraeEFStore(context);
                var purchaseStore = new PurchaseEFStore(context);
                var orgStore = new OrganisationsEFStore(context);
                var mockUserService = new Mock<IUserService>();
                mockUserService.Setup(o => o.ReadUserModelAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser());

                var permissionService = new PermissionService(permissionLogger, orgStore, amphoraStore);
                var options = Mock.Of<IOptionsMonitor<Api.Options.AmphoraManagementOptions>>(_ => _.CurrentValue == new Api.Options.AmphoraManagementOptions());
                var amphoraService = new AmphoraeService(options,
                                                         amphoraStore,
                                                         purchaseStore,
                                                         orgStore,
                                                         permissionService,
                                                         mockUserService.Object,
                                                         amphoraLogger);
                var service = new BasicSearchService(amphoraService);
                var orgModel = new OrganisationModel() { Name = "1234" };
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