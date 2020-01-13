namespace Amphora.GitHub.Models
{
    public class GitHubIssue
    {
        public GitHubIssue(string? body, string? title, string? htmlUrl)
        {
            Body = body;
            Title = title;
            HtmlUrl = htmlUrl;
        }

        /// <summary>
        ///  Details about the issue.
        /// </summary>
        public string? Body { get; protected set; }
        /// <summary>
        ///  Title of the issue
        /// </summary>
        public string? Title { get; protected set; }
        /// <summary>
        ///  The URL for the HTML view of this issue.
        /// </summary>
        public string? HtmlUrl { get; protected set; }
    }
}