namespace Amphora.GitHub.Models
{
    public class LinkedGitHubIssue : GitHubIssue
    {
        public LinkedGitHubIssue(string body, string title, string htmlUrl, LinkInformation linkInfo) : base(body, title, htmlUrl)
        {
            LinkInfo = linkInfo;
        }

        /// <summary>
        ///  Details about the issue.
        /// </summary>
        public LinkInformation? LinkInfo { get; protected set; }
    }
}