using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Westwind.AspNetCore.Markdown;

namespace Amphora.Api.Pages.Changelog
{
    public class IndexModel : PageModel
    {
        public IndexModel(IWebHostEnvironment env)
        {
            Env = env;
            ContentRootPath = Env.ContentRootPath;
        }
        public static string RootName = "wwwroot";
        public static string ChangelogsRelativePath = "docs/changelog";
        public IWebHostEnvironment Env { get; }
        public string ContentRootPath { get; }
        public Dictionary<string, string> VersionFiles { get; private set; }

        public IActionResult OnGetAsync()
        {
            var fullyQualifiedPaths = Directory.EnumerateFiles(Path.Join(ContentRootPath, RootName, ChangelogsRelativePath));
            var relativePaths = new Dictionary<string, string>(); // relative to wwwroot

            foreach(var f in fullyQualifiedPaths)
            {
                var version = Path.GetFileNameWithoutExtension(f).Replace('_', '.');
                var p = f.Substring(f.IndexOf(RootName) + RootName.Length );
                relativePaths.Add(version, p);
            }
            this.VersionFiles = relativePaths;
            return Page();
        }
    }
}