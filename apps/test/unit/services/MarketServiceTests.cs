using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Services.Amphorae;
using Amphora.Api.Services.Auth;
using Amphora.Api.Services.Basic;
using Amphora.Api.Services.Market;
using Amphora.Api.Stores.EFCore;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Users;
using Amphora.Tests.Helpers;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Amphora.Tests.Unit.Services
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
                var dataRequestStore = new DataRequestsEFStore(context, CreateMockLogger<DataRequestsEFStore>());
                var organisationStore = new OrganisationsEFStore(context, CreateMockLogger<OrganisationsEFStore>());
                var termsOfUseService = new TermsOfUseService();
                var userData = new ApplicationUserDataModel();
                var mockUserService = new Mock<IUserDataService>();
                mockUserService.Setup(o => o.ReadAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>()))
                    .ReturnsAsync(new Common.Models.EntityOperationResult<ApplicationUserDataModel>(userData, userData));

                var permissionService = new PermissionService(orgStore, amphoraStore, mockUserService.Object, CreateMockLogger<PermissionService>());
                var options = Mock.Of<IOptionsMonitor<Api.Options.AmphoraManagementOptions>>(_ => _.CurrentValue == new Api.Options.AmphoraManagementOptions());
                var amphoraService = new AmphoraeService(options,
                                                         amphoraStore,
                                                         purchaseStore,
                                                         orgStore,
                                                         termsOfUseService,
                                                         permissionService,
                                                         mockUserService.Object,
                                                         CreateMockEventPublisher(),
                                                         CreateMockLogger<AmphoraeService>());
                var service = new BasicSearchService(amphoraService, dataRequestStore, organisationStore);
                var orgModel = EntityLibrary.GetOrganisationModel();
                var amphora = EntityLibrary.GetAmphoraModel(orgModel); // dumy org id
                var entity = await amphoraStore.CreateAsync(amphora);
                var sut = new MarketService(service, amphoraService, CreateCache()) as IMarketService;

                var response = await sut.FindAsync("");
                Assert.NotNull(response);
                var entities = response.Results.Select(_ => _.Entity);
                Assert.NotEmpty(entities);
                Assert.Contains(entities, e => e.Id == entity.Id);
            }
        }
    }
}