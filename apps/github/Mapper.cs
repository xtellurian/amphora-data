using System.Collections.Generic;
using Amphora.GitHub.Models;
using Octokit;

namespace Amphora.GitHub
{
    internal static class Mapper
    {
        public static IReadOnlyList<GitHubIssue> ToGitHubIssue(IEnumerable<Issue> issues)
        {
            var result = new List<GitHubIssue>();
            foreach (var i in issues)
            {
                result.Add(ToGitHubIssue(i));
            }

            return result;
        }

        public static GitHubIssue ToGitHubIssue(Issue issue)
        {
            if (LinkInformation.TryParse(issue.Body, out var info))
            {
                return new LinkedGitHubIssue(issue.Body, issue.Title, issue.HtmlUrl, info!);
            }

            return new GitHubIssue(issue.Body, issue.Title, issue.HtmlUrl);
        }
    }
}