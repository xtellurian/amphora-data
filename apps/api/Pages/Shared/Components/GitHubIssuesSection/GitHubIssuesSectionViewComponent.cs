using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.GitHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Pages.Shared.Components
{
    [ViewComponent(Name = "GitHubIssuesSection")]
    public class GitHubIssuesSectionViewComponent : ViewComponent
    {
        private readonly IAmphoraGitHubIssueConnectorService issueService;

        public GitHubIssuesSectionViewComponent(IAmphoraGitHubIssueConnectorService issueService)
        {
            this.issueService = issueService;
        }

        public IReadOnlyList<LinkedGitHubIssue> Issues { get; private set; } = new List<LinkedGitHubIssue>();
        public string NewIssueUrl { get; private set; }

        public async Task<IViewComponentResult> InvokeAsync(AmphoraModel amphora)
        {
            if (amphora != null && amphora.Id != null)
            {
                this.Issues = await this.issueService.GetLinkedIssues(amphora.Id);
                this.NewIssueUrl = await this.issueService.NewIssueUrlAsync(amphora.Id);
            }

            return View(this);
        }
    }
}