using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Services.Amphorae;
using Amphora.Api.Stores.EFCore;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Permissions;
using Amphora.Common.Models.Signals;
using Amphora.Common.Services.Azure;
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
            var mockSender = new Mock<IEventHubSender>();
            var mockUserService = new Mock<IUserService>();
            var mockPermissionService = new Mock<IPermissionService>();
            mockPermissionService.Setup(_ => _.IsAuthorizedAsync(It.IsAny<IUser>(),
                                                                 It.IsAny<AmphoraModel>(),
                                                                 It.IsAny<AccessLevels>())).Returns(Task.FromResult(true));
            var mockTsi = new Mock<ITsiService>();
            var signalStore = new SignalsEFStore(context, CreateMockLogger<SignalsEFStore>());
            var mockPrincipal = new Mock<ClaimsPrincipal>();
            var sut = new SignalsService(mockSender.Object,
                                         mockUserService.Object,
                                         mockPermissionService.Object,
                                         mockTsi.Object,
                                         CreateMockLogger<SignalsService>());

            var amphora = new AmphoraModel();
            amphora.AddSignal(new SignalV2("numeric", SignalV2.Numeric));
            amphora.AddSignal(new SignalV2("string", SignalV2.String));

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
            var mockSender = new Mock<IEventHubSender>();
            var mockUserService = new Mock<IUserService>();
            var mockPermissionService = new Mock<IPermissionService>();
            mockPermissionService.Setup(_ => _.IsAuthorizedAsync(It.IsAny<IUser>(),
                                                                 It.IsAny<AmphoraModel>(),
                                                                 It.IsAny<AccessLevels>())).Returns(Task.FromResult(true));
            var mockTsi = new Mock<ITsiService>();
            var signalStore = new SignalsEFStore(context, CreateMockLogger<SignalsEFStore>());
            var mockPrincipal = new Mock<ClaimsPrincipal>();
            var sut = new SignalsService(mockSender.Object,
                                         mockUserService.Object,
                                         mockPermissionService.Object,
                                         mockTsi.Object,
                                         CreateMockLogger<SignalsService>());

            var amphora = new AmphoraModel();
            amphora.AddSignal(new SignalV2("numeric", SignalV2.Numeric));
            amphora.AddSignal(new SignalV2("string", SignalV2.String));

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