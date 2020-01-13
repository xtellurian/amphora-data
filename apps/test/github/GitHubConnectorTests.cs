using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Services.GitHub;
using Amphora.GitHub;
using Amphora.GitHub.Contracts;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Amphora.Tests.GitHub
{
    public class GitHubConnectorTests
    {
        private string token = System.Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        private string defaultUser = "xtellurian";
        private string defaultRepo = "mynewrepo";

        [Fact]
        public async Task CanGetNewIssueLink()
        {
            var configuration = new Configuration(System.Guid.NewGuid().ToString(), token, defaultUser, defaultRepo);
            IAmphoraGitHubClient sut = new AmphoraGitHubClient(configuration);

            var url = await sut.NewIssueUrl("blah&&&", "blah&&*?&");
            Assert.NotNull(url);
        }

        [Fact]
        public async Task CanGetListOfIssues()
        {
            var configuration = new Configuration(System.Guid.NewGuid().ToString(), token, defaultUser, defaultRepo);
            IAmphoraGitHubClient sut = new AmphoraGitHubClient(configuration);

            var issues = await sut.GetIssues();
            Assert.NotNull(issues);
            Assert.NotEmpty(issues);
        }

        [Fact]
        public async Task CanFilterListOfIssues()
        {
            var configuration = new Configuration(System.Guid.NewGuid().ToString(), token, defaultUser, defaultRepo);
            IAmphoraGitHubClient sut = new AmphoraGitHubClient(configuration);

            var id = System.Guid.NewGuid().ToString();
            var issues = await sut.GetIssues();
            Assert.NotNull(issues);
            var filtered = issues.Where(i => i.Body.Contains(id)).ToList();
            Assert.NotNull(filtered);
        }

        [Fact]
        public async Task CanGetIssueForEmptyGuid()
        {
            var config = new Configuration(System.Guid.NewGuid().ToString(), null, "xtellurian", "mynewrepo");
            var client = new AmphoraGitHubClient(config);
            var options = Mock.Of<IOptionsMonitor<Configuration>>(_ => _.CurrentValue == config);
            var sut = new AmphoraGitHubIssueConnectorService(options);
            // act
            var id = System.Guid.Empty.ToString();
            var issues = await sut.GetLinkedIssues(id);
            // assert
            Assert.NotNull(issues);
            Assert.NotEmpty(issues);
            var issue = issues.FirstOrDefault();
            Assert.True(issue.LinkInfo.AmphoraId == id);
        }
    }
}