using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.GitHub.Models;

namespace Amphora.Api.Contracts
{
    public interface IAmphoraGitHubIssueConnectorService
    {
        Task<IReadOnlyList<LinkedGitHubIssue>> GetLinkedIssues(string amphoraId);
    }
}