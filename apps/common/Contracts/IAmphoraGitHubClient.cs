using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Models.GitHub;

namespace Amphora.Common.Contracts
{
    public interface IAmphoraGitHubClient
    {
        Task<IReadOnlyList<GitHubIssue>> GetIssuesAsync(string? owner = null, string? repo = null);
        Task<string> NewIssueUrlAsync(string amphoraId, string title);
    }
}