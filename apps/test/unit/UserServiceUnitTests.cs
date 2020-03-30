using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Events;
using Amphora.Identity.Models;
using Amphora.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Amphora.Tests.Unit
{
    public class UserServiceUnitTests : UnitTestBase
    {
        private const string Password = "sodwkjdbvjkl@(*$&^q398";
        [Fact]
        public async Task NewUser_SendsEvent()
        {
            var email = Guid.NewGuid().ToString() + "@example.com";
            var appUser = new ApplicationUser
            {
                UserName = Guid.NewGuid().ToString() + "@example.com",
                Email = email
            };
            var mockUserManager = MockUserManager<ApplicationUser>(new List<ApplicationUser> { });
            mockUserManager.Setup(u => u.FindByNameAsync(appUser.UserName)).ReturnsAsync(appUser);

            var mock = new Mock<IEventPublisher>();
            mock.Setup(p => p.PublishEventAsync(It.IsAny<SignInEvent>())).Returns(Task.CompletedTask);

            var sut = new UserService(CreateMockLogger<UserService>(), mock.Object, mockUserManager.Object);

            var res = await sut.CreateAsync(appUser, null, Password);

            Assert.True(res.Succeeded);
            mock.Verify(_ => _.PublishEventAsync(It.IsAny<UserCreatedEvent>()), Times.Once());
        }
    }
}