using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Services.Amphorae;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Platform;
using Amphora.Common.Models.Users;
using Amphora.Common.Services.Plans;
using Amphora.Tests.Mocks;
using Moq;
using Xunit;

namespace Amphora.Tests.Unit.Services
{
    public class AmphoraFileServiceTests : UnitTestBase
    {
        [Fact]
        public async Task WhenLimitReached_FileUploadFails()
        {
            var principal = new TestPrincipal();
            var org = new OrganisationModel
            {
                Memberships = new List<Membership>
                {
                    new Membership(System.Guid.NewGuid().ToString())
                }
            };

            var userData = new ApplicationUserDataModel
            {
                UserName = "a username",
                Organisation = org
            };

            var planLimitService = new PlanLimitService();
            var mockBlobStore = new Mock<IBlobStore<AmphoraModel>>();
            var mockUserDataService = new Mock<IUserDataService>();
            mockUserDataService.Setup(_ => _.ReadAsync(principal, null))
                .ReturnsAsync(new EntityOperationResult<ApplicationUserDataModel>(userData, userData));

            org.Cache.TotalAmphoraeFileSize = new DataCache.CachedValue<long>(Units.TB); // TB is larger than the limit

            var amphora = new AmphoraModel
            {
                Organisation = org
            };

            var sut = new AmphoraFileService(CreateMockLogger<AmphoraFileService>(),
                planLimitService,
                CreateMockPermissionService(),
                mockBlobStore.Object,
                mockUserDataService.Object);

            var res = await sut.CreateFileAsync(principal, amphora, "something");
            Assert.False(res.Succeeded);
        }
    }
}