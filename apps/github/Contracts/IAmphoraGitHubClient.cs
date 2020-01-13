using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.GitHub.Models;

namespace Amphora.GitHub.Contracts
{
    public interface IAmphoraGitHubClient
    {
        Task<IReadOnlyList<GitHubIssue>> GetIssues(string? owner = null, string? repo = null);
        Task<string> NewIssueUrl(string title, string body);
    }
}