using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphora.Api.Pages.Changelog
{
    public class DetailModel : PageModel
    {
        public DetailModel(IWebHostEnvironment env)
        {
            Env = env;
            ContentRootPath = Env.ContentRootPath;
        }

        private string ChangelogsRelativePath => IndexModel.ChangelogsRelativePath;
        private string Rootname => IndexModel.RootName;
        private IWebHostEnvironment Env { get; }
        private string ContentRootPath { get; }
        public string Version { get; private set; }
        public string RelativePath { get; private set; }

        public IActionResult OnGet(string version)
        {
            if (!IsValidVersion(version))
            {
                return RedirectToPage("./Index");
            }

            this.Version = version;
            var fullPath = Path.Join(ContentRootPath, Rootname, ChangelogsRelativePath);
            var versionFiles = Directory.EnumerateFiles(fullPath);
            var fileName = version.Replace('.', '_');

            var absolutePath = versionFiles.FirstOrDefault(f => f.Contains(fileName));
            if (absolutePath == null)
            {
                return NotFound();
            }

            this.RelativePath = absolutePath.Substring(absolutePath.IndexOf(Rootname) + Rootname.Length);

            return Page();
        }

        private bool IsValidVersion(string v)
        {
            if (string.IsNullOrEmpty(v))
            {
                return false;
            }

            var arr = v.Split('.').ToList();
            if (arr.Count == 1)
            {
                // cannot have a version with no periods.
                return false;
            }

            return true;
        }
    }
}