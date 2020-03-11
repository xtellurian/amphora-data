using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Models.GitHub;

namespace Amphora.Api.Contracts
{
    public interface IAmphoraGitHubIssueConnectorService
    {
        Task<IReadOnlyList<LinkedGitHubIssue>> GetLinkedIssues(string amphoraId);
        Task<string> NewIssueUrlAsync(string amphoraId);
    }
}