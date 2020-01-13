using Amphora.GitHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Pages.Shared.Components
{
    [ViewComponent(Name = "GitHubIssueRow")]
    public class GitHubIssueRowViewComponent : ViewComponent
    {
        public GitHubIssue Issue { get; private set; }

        public IViewComponentResult Invoke(GitHubIssue issue)
        {
            this.Issue = issue;
            return View(this);
        }
    }
}