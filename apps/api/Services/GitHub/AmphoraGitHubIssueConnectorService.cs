using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.GitHub;
using Amphora.Infrastructure.Services.GitHub;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Services.GitHub
{
    public class AmphoraGitHubIssueConnectorService : IAmphoraGitHubIssueConnectorService
    {
        private const string IssuesMemoryCacheKey = "githubissues";
        private readonly IDateTimeProvider dtProvider;
        private readonly ICache cache;
        private AmphoraGitHubClient client;

        public AmphoraGitHubIssueConnectorService(IOptionsMonitor<GitHubConfiguration> config,
                                                  IDateTimeProvider dtProvider,
                                                  ICache cache = null)
        {
            this.client = new AmphoraGitHubClient(config.CurrentValue);
            this.dtProvider = dtProvider;
            this.cache = cache;
        }

        public async Task<IReadOnlyList<LinkedGitHubIssue>> GetLinkedIssues(string amphoraId)
        {
            if (!TryGetCachedIssues(out var issues))
            {
                cache?.Compact(50);
                // load from GitHub
                issues = await client.GetLinkedIssues();
                TryCacheIssues(issues);
            }

            var forThisId = issues.Where(_ => _.LinkInfo.AmphoraId == amphoraId);
            return new List<LinkedGitHubIssue>(forThisId);
        }

        public async Task<string> NewIssueUrlAsync(string amphoraId)
        {
            return await this.client.NewIssueUrlAsync(amphoraId, null);
        }

        private bool TryGetCachedIssues(out IReadOnlyList<LinkedGitHubIssue> issues)
        {
            if (cache == null)
            {
                issues = new List<LinkedGitHubIssue>();
                return false;
            }

            return cache.TryGetValue(IssuesMemoryCacheKey, out issues);
        }

        private void TryCacheIssues(IReadOnlyList<LinkedGitHubIssue> issues)
        {
            if (cache == null) { return; }
            // Save data in cache.
            cache.Set(IssuesMemoryCacheKey, issues, System.TimeSpan.FromSeconds(5));
        }
    }
}