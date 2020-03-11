namespace Amphora.Common.Models.GitHub
{
    public class LinkedGitHubIssue : GitHubIssue
    {
        public LinkedGitHubIssue(int id, string body, string title, string htmlUrl, LinkInformation linkInfo) : base(id, body, title, htmlUrl)
        {
            LinkInfo = linkInfo;
        }

        /// <summary>
        ///  Details about the issue.
        /// </summary>
        public LinkInformation? LinkInfo { get; protected set; }
    }
}