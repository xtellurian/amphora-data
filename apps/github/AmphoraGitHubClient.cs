using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<string> NewIssueUrlAsync(string amphoraId, string title)
        {
            var repo = await GetRepoAsync();
            var body = LinkInformation.Template(amphoraId);
            var sb = new StringBuilder(repo.HtmlUrl).Append("/issues/new?");
            if (title != null) { sb.Append($"title={title}"); }
            sb.Append($"&body={body}");
            return sb.ToString();
        }

        public async Task<IReadOnlyList<GitHubIssue>> GetIssuesAsync(string? owner = null, string? repo = null)
        {
            DefaultIfNull(ref owner, ref repo);
            var issues = await client.Issue.GetAllForRepository(owner, repo);
            return Mapper.ToGitHubIssue(issues);
        }

        public async Task<IReadOnlyList<LinkedGitHubIssue>> GetLinkedIssues(string? owner = null, string? repo = null)
        {
            var issues = await this.GetIssuesAsync(owner, repo);
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
