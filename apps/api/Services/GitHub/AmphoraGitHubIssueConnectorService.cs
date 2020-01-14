using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.GitHub;
using Amphora.GitHub.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Services.GitHub
{
    public class AmphoraGitHubIssueConnectorService : IAmphoraGitHubIssueConnectorService
    {
        private const string IssuesMemoryCacheKey = "githubissues";
        private readonly IDateTimeProvider dtProvider;
        private readonly IMemoryCache cache;
        private AmphoraGitHubClient client;

        public AmphoraGitHubIssueConnectorService(IOptionsMonitor<Amphora.GitHub.Configuration> config,
                                                  IDateTimeProvider dtProvider,
                                                  IMemoryCache cache = null)
        {
            this.client = new AmphoraGitHubClient(config.CurrentValue);
            this.dtProvider = dtProvider;
            this.cache = cache;
        }

        public async Task<IReadOnlyList<LinkedGitHubIssue>> GetLinkedIssues(string amphoraId)
        {
            if (!TryGetCachedIssues(out var issues))
            {
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
            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(System.TimeSpan.FromSeconds(5));

            // Save data in cache.
            cache.Set(IssuesMemoryCacheKey, issues, cacheEntryOptions);
        }
    }
}