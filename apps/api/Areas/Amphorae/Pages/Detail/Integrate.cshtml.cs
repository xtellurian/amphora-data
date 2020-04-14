using System.IO;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Areas.Amphorae.Pages.Detail
{
    [CommonAuthorize]
    public class IntegratePageModel : AmphoraDetailPageModel
    {
        public IntegratePageModel(IAmphoraeService amphoraeService,
                              IQualityEstimatorService qualityEstimator,
                              IPurchaseService purchaseService,
                              IPermissionService permissionService,
                              IWebHostEnvironment env) : base(amphoraeService, qualityEstimator, purchaseService, permissionService)
        {
            this.ContentRootPath = env.ContentRootPath;
        }

        private static string rootName = "wwwroot";
        private static string snippetsRelativePath = "docs/snippets";
        public MarkdownCodeSnippets Snippets { get; set; }
        public string ContentRootPath { get; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await LoadAmphoraAsync(id);
            await SetPagePropertiesAsync();
            return OnReturnPage();
        }

        protected override async Task SetPagePropertiesAsync()
        {
            await base.SetPagePropertiesAsync();
            var pythonSnippetPath = Path.Join(rootName, snippetsRelativePath, "python.md");
            this.Snippets = new MarkdownCodeSnippets(Amphora.Id, Result.User.UserName, pythonSnippetPath);
        }

        public class MarkdownCodeSnippets
        {
            private readonly string id;
            private readonly string userName;
            private readonly string pythonPath;

            public MarkdownCodeSnippets(string id, string userName, string pythonPath)
            {
                this.id = id;
                this.userName = userName;
                this.pythonPath = pythonPath;
            }

            public string Python => System.IO.File.ReadAllText(pythonPath)
                .Replace("{{id}}", id)
                .Replace("{{username}}", userName);
        }
    }
}