using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Services.Transactions;
using Amphora.Api.Stores.EFCore;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Users;
using Moq;
using Xunit;

namespace Amphora.Tests.Unit.Purchasing
{
    public class PurchaseServiceTests : UnitTestBase
    {
        [Fact]
        public async Task PurchasingAmphora_SendsEmail()
        {
            var context = base.GetContext();
            var store = new PurchaseEFStore(context);
            var user = new ApplicationUser()
            {
                Email = "test@amphoradata.com"
            };
            var amphora = new AmphoraModel();
            amphora.Id = System.Guid.NewGuid().ToString();
            var mockEmailSender = new Mock<IEmailSender>();
            var logger = CreateMockLogger<PurchaseService>();

            mockEmailSender.Setup(_ => _.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var sut = new PurchaseService(store, null, mockEmailSender.Object, logger);

            var result = await sut.PurchaseAmphora(user, amphora);
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Entity);

            mockEmailSender.Verify(mock => mock.SendEmailAsync(It.Is<string>(s => s == user.Email), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }
    }
}