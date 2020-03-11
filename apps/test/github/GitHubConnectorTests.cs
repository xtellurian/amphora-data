using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amphora.Api.Services.GitHub;
using Amphora.Common.Contracts;
using Amphora.Common.Models.GitHub;
using Amphora.Infrastructure.Services.GitHub;
using Amphora.Tests.Mocks;
using Bogus;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Amphora.Tests.GitHub
{
    public class GitHubConnectorTests
    {
        private Faker faker = new Faker("en");

        private string token = System.Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        private string defaultUser = "xtellurian";
        private string defaultRepo = "mynewrepo";

        [Fact]
        public async Task CanGetNewIssueLink()
        {
            var configuration = new GitHubConfiguration(System.Guid.NewGuid().ToString(), token, defaultUser, defaultRepo);
            IAmphoraGitHubClient sut = new AmphoraGitHubClient(configuration);
            var id = System.Guid.NewGuid().ToString();
            var url = await sut.NewIssueUrlAsync(id, "blah&&&");
            Assert.NotNull(url);
        }

        [Fact]
        public async Task CanGetListOfIssues()
        {
            var configuration = new GitHubConfiguration(System.Guid.NewGuid().ToString(), token, defaultUser, defaultRepo);
            IAmphoraGitHubClient sut = new AmphoraGitHubClient(configuration);

            var issues = await sut.GetIssuesAsync();
            Assert.NotNull(issues);
            Assert.NotEmpty(issues);
        }

        [Fact]
        public async Task CanFilterListOfIssues()
        {
            var configuration = new GitHubConfiguration(System.Guid.NewGuid().ToString(), token, defaultUser, defaultRepo);
            IAmphoraGitHubClient sut = new AmphoraGitHubClient(configuration);

            var id = System.Guid.NewGuid().ToString();
            var issues = await sut.GetIssuesAsync();
            Assert.NotNull(issues);
            var filtered = issues.Where(i => i.Body.Contains(id)).ToList();
            Assert.NotNull(filtered);
        }

        [Fact]
        public async Task CanGetIssueForEmptyGuid()
        {
            var dtProvider = new MockDateTimeProvider();
            var config = new GitHubConfiguration(System.Guid.NewGuid().ToString(), null, "xtellurian", "mynewrepo");
            var client = new AmphoraGitHubClient(config);
            var options = Mock.Of<IOptionsMonitor<GitHubConfiguration>>(_ => _.CurrentValue == config);
            var sut = new AmphoraGitHubIssueConnectorService(options, dtProvider);
            // act
            var id = System.Guid.Empty.ToString();
            var issues = await sut.GetLinkedIssues(id);
            // assert
            Assert.NotNull(issues);
            Assert.NotEmpty(issues);
            var issue = issues.FirstOrDefault();
            Assert.True(issue.LinkInfo.AmphoraId == id);
        }

        [Fact]
        public async Task CanGenerateCreateIssueUrl()
        {
            var config = new GitHubConfiguration(System.Guid.NewGuid().ToString(), null, "xtellurian", "mynewrepo");
            var client = new AmphoraGitHubClient(config);
            var id = System.Guid.NewGuid().ToString();
            var url = await client.NewIssueUrlAsync(id, faker.Rant.Review("amphora"));
            using (var httpClient = new HttpClient())
            {
                var res = await httpClient.GetAsync(url);
                Assert.True(res.IsSuccessStatusCode);
            }
        }
    }
}