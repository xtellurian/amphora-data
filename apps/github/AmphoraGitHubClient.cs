using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.GitHub.Contracts;
using Amphora.GitHub.Models;
using Octokit;

namespace Amphora.GitHub
{
    public class AmphoraGitHubClient : IAmphoraGitHubClient
    {
        private GitHubClient client;
        private Configuration config;

        public AmphoraGitHubClient(Configuration config)
        {
            this.client = new GitHubClient(new ProductHeaderValue(config.ProductHeaderValue));
            if (!string.IsNullOrEmpty(config.Token)) { this.client.Credentials = new Credentials(config.Token); }
            this.config = config;
        }

        internal async Task<Repository> GetRepoAsync(string? owner = null, string? repo = null)
        {
            DefaultIfNull(ref owner, ref repo);
            return await client.Repository.Get(owner, repo);
        }

        public async Task<string> NewIssueUrl(string title, string body)
        {
            var repo = await GetRepoAsync();
            var titleHtml = System.Web.HttpUtility.HtmlEncode(title);
            var bodyHtml = System.Web.HttpUtility.HtmlEncode(body);
            return $"{repo.HtmlUrl}/issues/new?title={titleHtml}&body={bodyHtml}";
        }

        public async Task<IReadOnlyList<GitHubIssue>> GetIssues(string? owner = null, string? repo = null)
        {
            DefaultIfNull(ref owner, ref repo);
            var issues = await client.Issue.GetAllForRepository(owner, repo);
            return Mapper.ToGitHubIssue(issues);
        }

        public async Task<IReadOnlyList<LinkedGitHubIssue>> GetLinkedIssues(string? owner = null, string? repo = null)
        {
            var issues = await this.GetIssues(owner, repo);
            return issues
                .Where(_ => _ is LinkedGitHubIssue)
                .Cast<LinkedGitHubIssue>()
                .ToList();
        }

        public async Task DoThing()
        {
            var user = await client.User.Get("xtellurian");
            var org = await client.Organization.Get("AmphoraData");
            var repo = await client.Repository.Get("xtellurian", "amphoradata.github.io");
            var issues = await client.Issue.GetAllForRepository(config.DefaultUser, config.DefaultRepo);
            Console.WriteLine("{0} has {1} public repositories - go check out their profile at {2}",
                user.Name,
                user.PublicRepos,
                user.Url);
        }

        private void DefaultIfNull(ref string? gitHubUser, ref string? repo)
        {
            if (gitHubUser == null)
            {
                gitHubUser = config.DefaultUser;
            }

            if (repo == null)
            {
                repo = config.DefaultRepo;
            }
        }
    }
}
