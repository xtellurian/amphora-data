using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.GitHub;
using Amphora.GitHub.Models;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Services.GitHub
{
    public class AmphoraGitHubIssueConnectorService : IAmphoraGitHubIssueConnectorService
    {
        private AmphoraGitHubClient client;

        public AmphoraGitHubIssueConnectorService(IOptionsMonitor<Amphora.GitHub.Configuration> config)
        {
            this.client = new AmphoraGitHubClient(config.CurrentValue);
        }

        public async Task<IReadOnlyList<LinkedGitHubIssue>> GetLinkedIssues(string amphoraId)
        {
            var issues = await client.GetLinkedIssues();
            var forThisId = issues.Where(_ => _.LinkInfo.AmphoraId == amphoraId);
            return new List<LinkedGitHubIssue>(forThisId);
        }

        public async Task<string> NewIssueUrlAsync(string amphoraId)
        {
            return await this.client.NewIssueUrlAsync(amphoraId, null);
        }
    }
}