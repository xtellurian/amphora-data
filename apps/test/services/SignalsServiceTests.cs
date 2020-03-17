using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Services.Amphorae;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Permissions;
using Amphora.Common.Models.Users;
using Moq;
using Xunit;

namespace Amphora.Tests.Unit.Services
{
    public class SignalsServiceTests : UnitTestBase
    {
        public SignalsServiceTests()
        { }

        [Fact]
        public async Task UploadStringToNumericSignal_ReturnsError()
        {
            var context = GetContext();
            var fakeUserData = new ApplicationUserDataModel
            {
                Id = System.Guid.NewGuid().ToString()
            };
            var mockSender = new Mock<IEventHubSender>();
            var mockUserDataService = new Mock<IUserDataService>();
            mockUserDataService
                .Setup(_ => _.ReadAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>()))
                .ReturnsAsync(new EntityOperationResult<ApplicationUserDataModel>(fakeUserData, fakeUserData));
            var mockPermissionService = new Mock<IPermissionService>();
            mockPermissionService.Setup(_ => _.IsAuthorizedAsync(It.IsAny<IUser>(),
                                                                 It.IsAny<AmphoraModel>(),
                                                                 It.IsAny<AccessLevels>())).Returns(Task.FromResult(true));
            var mockTsi = new Mock<ITsiService>();
            var mockPrincipal = new Mock<ClaimsPrincipal>();
            var sut = new SignalsService(mockSender.Object,
                                         mockUserDataService.Object,
                                         mockPermissionService.Object,
                                         mockTsi.Object,
                                         CreateMockLogger<SignalsService>());

            var amphora = new AmphoraModel();
            Assert.True(amphora.TryAddSignal(new SignalV2("numeric", SignalV2.Numeric), out var m1));
            Assert.True(amphora.TryAddSignal(new SignalV2("string", SignalV2.String), out var m2));

            var data = new Dictionary<string, object>()
            {
                { "string", "a regular string" },
                { "numeric", "an unexpected string" }
            };
            var res = await sut.WriteSignalAsync(mockPrincipal.Object, amphora, data);
            Assert.False(res.WasForbidden); // make sure this isn't a permissions test
            Assert.False(res.Succeeded);
            var data_ok = new Dictionary<string, object>()
            {
                { "string", "a regular string" },
                { "numeric", 2 }
            };
            var res_ok = await sut.WriteSignalAsync(mockPrincipal.Object, amphora, data_ok);
            Assert.False(res_ok.WasForbidden); // make sure this isn't a permissions test
            Assert.True(res_ok.Succeeded);
            Assert.False(string.IsNullOrEmpty(res.Message));
        }

        [Fact]
        public async Task UploadSignalWithIncorrectCase_ReturnsError()
        {
            var context = GetContext();
            var fakeUserData = new ApplicationUserDataModel
            {
                Id = System.Guid.NewGuid().ToString()
            };
            var mockSender = new Mock<IEventHubSender>();
            var mockUserDataService = new Mock<IUserDataService>();
            mockUserDataService
                .Setup(_ => _.ReadAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>()))
                .ReturnsAsync(new EntityOperationResult<ApplicationUserDataModel>(fakeUserData, fakeUserData));

            var mockPermissionService = new Mock<IPermissionService>();
            mockPermissionService.Setup(_ => _.IsAuthorizedAsync(It.IsAny<IUser>(),
                                                                 It.IsAny<AmphoraModel>(),
                                                                 It.IsAny<AccessLevels>())).Returns(Task.FromResult(true));
            var mockTsi = new Mock<ITsiService>();
            var mockPrincipal = new Mock<ClaimsPrincipal>();
            var sut = new SignalsService(mockSender.Object,
                                         mockUserDataService.Object,
                                         mockPermissionService.Object,
                                         mockTsi.Object,
                                         CreateMockLogger<SignalsService>());

            var amphora = new AmphoraModel();
            amphora.TryAddSignal(new SignalV2("numeric", SignalV2.Numeric), out var message);
            amphora.TryAddSignal(new SignalV2("string", SignalV2.String), out var message2);

            var data = new Dictionary<string, object>()
            {
                { "STRING", "a regular string" },
                { "NUMERIC", 7 }
            };
            var res = await sut.WriteSignalAsync(mockPrincipal.Object, amphora, data);
            Assert.False(res.WasForbidden); // make sure this isn't a permissions test
            Assert.False(res.Succeeded);
            Assert.False(string.IsNullOrEmpty(res.Message));
        }
    }
}