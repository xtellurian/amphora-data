using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Feeds;
using Amphora.Api.Services.Feeds;
using Amphora.Common.Models.Organisations;
using Amphora.Common.Models.Users;
using Amphora.Tests.Helpers;
using Amphora.Tests.Mocks;
using FluentAssertions;
using Moq;
using Xunit;

namespace Amphora.Tests.Unit.Services
{
    public class FeedAggregatorTests : UnitTestBase
    {
        [Fact]
        public async Task FeedContainsPosts()
        {
            var org = EntityLibrary.GetOrganisationModel();
            var amphora = EntityLibrary.GetAmphoraModel(org);
            var posts = new List<IPost>
            {
                new AmphoraCreatedPost(amphora)
            };
            var userData = new ApplicationUserDataModel
            {
                Id = Guid.NewGuid().ToString()
            };
            var testPrincipal = new TestPrincipal();
            var mockUserDataService = CreateMockUserDataService(new ConnectUser(userData, testPrincipal));
            var mockAmphoraFeedService = new Mock<IAmphoraFeedService>();
            mockAmphoraFeedService.Setup(_ => _.GetPostsAsync(It.IsAny<OrganisationModel>(), It.IsAny<int>())).ReturnsAsync(posts);

            var sut = new FeedAggregatorService(mockUserDataService.Object, mockAmphoraFeedService.Object);

            var feedResult = await sut.GetFeedAsync(testPrincipal);
            feedResult.Succeeded.Should().BeTrue();
            feedResult.Entity.Items.Should().NotBeEmpty();
        }
    }
}